using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Events;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.EventHandlers;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.EmailNotification;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;

namespace Tranglo1.CustomerIdentity.IdentityServer.CustomerUserList.EventHandlers
{
    class CustomerUserRegisteredEventHandler : BaseEventHandler<CustomerUserRegisteredEvent>  //: INotificationHandler<DomainEventNotification<CustomerUserRegisteredEvent>>
    {
        private readonly ILogger<CustomerUserRegisteredEventHandler> _logger;
        private readonly TrangloUserManager _userManager;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;
        private readonly IBusinessProfileRepository _businessProfileRepository;

        public CustomerUserRegisteredEventHandler(
            ILogger<CustomerUserRegisteredEventHandler> logger, TrangloUserManager userManager,
            INotificationService notificationService, IConfiguration config,
            IWebHostEnvironment environment, IBusinessProfileRepository businessProfileRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _notificationService = notificationService;
            _config = config;
            _environment = environment;
            _businessProfileRepository = businessProfileRepository;
        }

        protected override async Task HandleAsync(CustomerUserRegisteredEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                var _Customer = await this._userManager.FindByIdAsync(@event.Email);

                if (_Customer != null)
                {
                    _logger.LogDebug($"Generating confirmation email for {@event.Email}.");

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(_Customer);
                    UserVerificationToken userVerificationToken = new UserVerificationToken()
                    {
                        Email = @event.Email,
                        Token = token
                    };
                    await _businessProfileRepository.AddUserVerificationToken(userVerificationToken);
                    var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                    string confirmationUrl = $"{_config.GetValue<string>("IdentityServerUri")}/account/confirmemail/?userid={@event.Email}&Token={code}";
                    string confirmationPlaceHolder = Guid.NewGuid().ToString();
                    var customerRegisterEmailSubject = "";
                    string solution = String.Empty;


                    if (@event.IsMultipleSolutions)
                    {
                        customerRegisterEmailSubject = "(Tranglo Connect + Tranglo Business) Sign up verification";
                        solution = "Tranglo Connect + Tranglo Business";
                    }
                    else
                    {
                        // 1. get solution environment
                        if (@event.SolutionCode == Solution.Connect.Id)
                        {
                            customerRegisterEmailSubject = "(Tranglo Connect) Sign up verification";
                            solution = Solution.Connect.Name;
                        }
                        else if (@event.SolutionCode == Solution.Business.Id)
                        {
                            customerRegisterEmailSubject = "(Tranglo Business) Sign up verification";
                            solution = Solution.Business.Name;
                        }
                    }

                    var recipients = new List<RecipientsInputDTO>
                    {
                        new RecipientsInputDTO()
                        {
                            email = @event.Email,
                            name = @event.FullName
                        }
                    };

                    var cc = new List<RecipientsInputDTO>();

                    var bcc = new List<RecipientsInputDTO>();


                    EmailNotificationInputDTO emailNotificationInputDTO = new EmailNotificationInputDTO();
                    emailNotificationInputDTO.recipients = recipients;
                    emailNotificationInputDTO.cc = cc;
                    emailNotificationInputDTO.bcc = bcc;
                    emailNotificationInputDTO.RecipientName = @event.FullName;
                    emailNotificationInputDTO.ConfirmationPlaceHolder = confirmationPlaceHolder;
                    emailNotificationInputDTO.Url = confirmationUrl;
                    emailNotificationInputDTO.NotificationTemplate = "VerifyEmailTemplate";
                    emailNotificationInputDTO.NotificationType = NotificationTypes.Email;
                    emailNotificationInputDTO.subject = customerRegisterEmailSubject;
                    emailNotificationInputDTO.SolutionName = solution;

                    var sendEmailResponse = await _notificationService.SendNotification(emailNotificationInputDTO);

                    if (sendEmailResponse.IsFailure)
                    {
                        _logger.LogError("SendNotification", $"[CustomerUserRegisteredEventHandler] {customerRegisterEmailSubject} Notification failed for {@event.FullName}. {sendEmailResponse.Error}.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[CustomerUserRegisteredEventHandler] {ex.ToString()}");
            }
        }
    }
}
