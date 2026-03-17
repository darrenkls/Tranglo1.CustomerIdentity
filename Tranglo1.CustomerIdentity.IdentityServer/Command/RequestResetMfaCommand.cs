using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.EmailNotification;
using Tranglo1.CustomerIdentity.IdentityServer.Helper;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    public class RequestResetMfaCommand : BaseCommand<Result>
    {
        public ApplicationUser ApplicationUser { get; set; }

        public class RequestResetMfaCommandHandler : IRequestHandler<RequestResetMfaCommand, Result>
        {
            private readonly TrangloUserManager _userManager;
            private readonly IApplicationUserRepository _applicationUserRepository;
            private readonly IBusinessProfileRepository _businessProfileRepository;
            private readonly INotificationService _notificationService;
            private readonly IConfiguration _configuration;
            private readonly ILogger<RequestResetMfaCommandHandler> _logger;

            public RequestResetMfaCommandHandler(TrangloUserManager userManager,
                IApplicationUserRepository applicationUserRepository,
                IBusinessProfileRepository businessProfileRepository,
                INotificationService notificationService,
                IConfiguration configuration,
                ILogger<RequestResetMfaCommandHandler> logger)
            {
                _userManager = userManager;
                _applicationUserRepository = applicationUserRepository;
                _businessProfileRepository = businessProfileRepository;
                _notificationService = notificationService;
                _configuration = configuration;
                _logger = logger;
            }

            public async Task<Result> Handle(RequestResetMfaCommand request, CancellationToken cancellationToken)
            {
                var user = request.ApplicationUser;

                var hasEnabled2FA = await _userManager.GetTwoFactorEnabledAsync(user);
                if (!hasEnabled2FA)
                {
                    return Result.Failure("Could not disable 2FA for user as it was not enabled.");
                }

                string token;
                bool isUniqueToken = false;

                #region Generate Unique Token
                do
                {
                    token = SecurityStringGenerator.GenerateRandomString(20).ToUpperInvariant();

                    var mfaResetRequest = await _applicationUserRepository.GetMFAResetRequestByTokenAsync(token);
                    if (mfaResetRequest == null)
                        isUniqueToken = true;
                } while (!isUniqueToken);
                #endregion

                string reset2FAUri = $"{_configuration.GetValue<string>("IdentityServerUri")}/Account/MFA/Reset/{token}";

                var newMFAResetRequest = new MFAResetRequest
                {
                    UserId = user.Id,
                    Token = token,
                };
                await _applicationUserRepository.InsertMFAResetRequestAsync(newMFAResetRequest);

                #region Send Reset 2FA Request Email for Customer
                if (user is CustomerUser customerUser)
                {
                    var partnerSubscriptions = await _userManager.GetPartnerSubscriptionsForUserAsync(customerUser);
                    var partnerSubscriptionsGroupedBySolutionCode = partnerSubscriptions
                        .GroupBy(x => x.SolutionCode)
                        .Select(g => new
                        {
                            SolutionCode = g.Key,
                            PartnerSubscriptions = g.ToList()
                        })
                        .ToList();

                    foreach (var solutionGroup in partnerSubscriptionsGroupedBySolutionCode)
                    {
                        #region Send Reset 2FA Request Email
                        string solutionName = String.Empty;
                        if (solutionGroup.SolutionCode.HasValue)
                        {
                            var solution = Enumeration.FindById<Solution>(solutionGroup.SolutionCode.Value);
                            solutionName = solution.Name;
                        }

                        string entityName = String.Empty;
                        var entity = solutionGroup.PartnerSubscriptions
                            .Where(x => !String.IsNullOrEmpty(x.TrangloEntity))
                            .OrderBy(x => x.Id)
                            .FirstOrDefault()?.TrangloEntity;
                        if (entity != null)
                        {
                            entityName = TrangloEntity.GetByEntityByTrangloId(entity)?.Name;
                        }

                        Result sendEmailResult = await SendReset2FARequestEmailAsync(user, reset2FAUri, solutionName, entityName);
                        if (sendEmailResult.IsFailure)
                        {
                            return sendEmailResult;
                        }
                        #endregion Send Reset 2FA Request Email
                    }
                }
                #endregion
                #region Send Reset 2FA Request Email for Non-Customer
                else
                {
                    Result sendEmailResult = await SendReset2FARequestEmailAsync(user, reset2FAUri, null, null);
                    if (sendEmailResult.IsFailure)
                    {
                        return sendEmailResult;
                    }
                }
                #endregion Send Reset 2FA Request Email for Non-Customer

                return Result.Success();
            }

            private async Task<Result> SendReset2FARequestEmailAsync(ApplicationUser user,
                string reset2FAUri,
                string solutionName,
                string entityName)
            {
                NotificationTemplate notificationTemplate = NotificationTemplate.Reset2faRequest;
                string subject = "Reset two-factor authentication (2FA)";
                if (!String.IsNullOrEmpty(solutionName))
                    subject = $"({solutionName}) {subject}";

                List<EmailRecipient> bccList = await _businessProfileRepository.GetRecipientEmail(RecipientType.BCC.Id, notificationTemplate.Id);
                List<EmailRecipient> ccList = await _businessProfileRepository.GetRecipientEmail(RecipientType.CC.Id, notificationTemplate.Id);

                EmailNotificationInputDTO emailNotificationInputDTO = new EmailNotificationInputDTO
                {
                    recipients = new List<RecipientsInputDTO>
                    {
                        new RecipientsInputDTO
                        {
                            email = user.Email.Value,
                            name = user.FullName.Value
                        }
                    },
                    cc = ccList.Select(x => new RecipientsInputDTO { email = x.Email, name = x.Name })
                        .ToList(),
                    bcc = bccList.Select(x => new RecipientsInputDTO { email = x.Email, name = x.Name })
                        .ToList(),
                    subject = subject,
                    Module = "CustomerIdentity",
                    SubModule = "Request Reset 2FA",
                    NotificationType = NotificationTypes.Email,
                    NotificationTemplate = notificationTemplate.Name + "Template",
                    SolutionName = solutionName,
                    RecipientName = user.FullName.Value,
                    Entity = entityName,
                    Reset2FAUri = reset2FAUri
                };

                var sendEmailResponse = await _notificationService.SendNotification(emailNotificationInputDTO);
                if (sendEmailResponse.IsFailure)
                {
                    _logger.LogError("Sending Email for Reset 2FA Request is FAILED! [{Error}]", sendEmailResponse.Error);
                    return Result.Failure("Fail to send the Reset two-factor authentication (2FA) email. Please contact the support.");
                }

                return Result.Success();
            }
        }
    }
}
