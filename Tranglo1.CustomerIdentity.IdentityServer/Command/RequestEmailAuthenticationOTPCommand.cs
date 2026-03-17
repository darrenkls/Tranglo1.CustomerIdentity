using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.EmailNotification;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    public class RequestEmailAuthenticationOTPCommand : BaseCommand<Result<bool>>
    {
        public string LoginId { get; set; }

        internal class RequestEmailAuthenticationOTPCommandHandler : IRequestHandler<RequestEmailAuthenticationOTPCommand, Result<bool>>
        {
            private readonly INotificationService _notificationService;
            private readonly IConfiguration _config;
            private readonly IWebHostEnvironment _environment;
            private readonly IApplicationUserRepository _repository;
            private readonly TrangloUserManager _userManager;
            private readonly IBusinessProfileRepository _businessProfileRepository;
            private readonly IPartnerRepository _partnerRepository;
            private readonly ILogger<RequestEmailAuthenticationOTPCommandHandler> _logger;

            public RequestEmailAuthenticationOTPCommandHandler(
                INotificationService notificationService,
                IConfiguration configuration,
                IWebHostEnvironment environment,
                IApplicationUserRepository repository,
                TrangloUserManager userManager,
                IBusinessProfileRepository businessProfileRepository,
                IPartnerRepository partnerRepository,
                ILogger<RequestEmailAuthenticationOTPCommandHandler> logger
                )
            {
                _notificationService = notificationService;
                _config = configuration;
                _environment = environment;
                _repository = repository;
                _userManager = userManager;
                _businessProfileRepository = businessProfileRepository;
                _partnerRepository = partnerRepository;
                _logger = logger;
            }

            public async Task<Result<bool>> Handle(RequestEmailAuthenticationOTPCommand request, CancellationToken cancellationToken)
            {
                var xsltTemplateRootPath = Path.Combine(_environment.ContentRootPath, "templates/emailtemplate");
                string cultureName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                var generator = new Common.ContentGenerator(xsltTemplateRootPath);
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(request.LoginId);
                MFAEmailOTP otp = new MFAEmailOTP();
                var randNum = System.Security.Cryptography.RandomNumberGenerator.GetInt32(999999);
                otp.OTP = randNum.ToString().PadLeft(6, '0');
                otp.Email = applicationUser.Email.Value;
                otp.LoginID = request.LoginId;

                await _repository.NewMFAEMailOTPAsync(otp);

                var _customerUserBusinessProfile = await _businessProfileRepository.GetCustomerUserBusinessProfilesByUserIdAsync(applicationUser.Id);
                var partnerRegistration = await _partnerRepository.GetPartnerRegistrationByBusinessProfileCodeAsync(_customerUserBusinessProfile.BusinessProfileCode);
                var partnerSubInfo = await _partnerRepository.GetPartnerSubscriptionListAsync(partnerRegistration.Id);
                bool isTrangloConnect = partnerSubInfo.Any(x => x.Solution == Solution.Connect);
                bool isTrangloBusiness = partnerSubInfo.Any(x => x.Solution == Solution.Business);

                string entity = string.Empty;

                if (isTrangloConnect && isTrangloBusiness)
                {
                    entity = "(Tranglo Connect + Tranglo Business) ";
                }
                else if (isTrangloConnect)
                {
                    entity = "(Tranglo Connect) ";
                }
                else if (isTrangloBusiness)
                {
                    entity = "(Tranglo Business) ";
                }

                var recipients = new List<RecipientsInputDTO>()
                {
                    new RecipientsInputDTO
                    {
                        email = applicationUser.Email.Value,
                        name = applicationUser.FullName.Value
                    }
                };
                var cc = new List<RecipientsInputDTO>();
                var bcc = new List<RecipientsInputDTO>();

                string subject = $"{entity}Login OTP verification";

                EmailNotificationInputDTO emailNotificationInputDTO = new EmailNotificationInputDTO();
                emailNotificationInputDTO.recipients = recipients;
                emailNotificationInputDTO.cc = cc;
                emailNotificationInputDTO.bcc = bcc;
                emailNotificationInputDTO.RecipientName = applicationUser.FullName?.Value;
                emailNotificationInputDTO.Otp = otp.OTP;
                emailNotificationInputDTO.NotificationTemplate = "MFARequestEmailOTPTemplate";
                emailNotificationInputDTO.NotificationType = NotificationTypes.Email;
                emailNotificationInputDTO.subject = subject;

                var sendEmailResponse = await _notificationService.SendNotification(emailNotificationInputDTO);

                if (sendEmailResponse.IsFailure)
                {
                    _logger.LogError("SendNotification", $"[RequestEmailAuthenticationOTPCommand] Login OTP Notification failed for {applicationUser.FullName?.Value}. {sendEmailResponse.Error}.");
                }

                return Result.Success(true);
            }
        }
    }
}
