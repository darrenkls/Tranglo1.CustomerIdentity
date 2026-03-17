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
using System.Linq;

namespace Tranglo1.CustomerIdentity.IdentityServer.EventHandlers
{
    class CustomerUserInvitationSubmittedEventHandler : BaseEventHandler<CustomerUserInvitationSubmittedEvent>
    {
        private readonly ILogger<CustomerUserInvitationSubmittedEventHandler> _logger;
        private readonly TrangloUserManager _userManager;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _config;
        private readonly IBusinessProfileRepository _businessProfileRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IPartnerRepository _partnerRepository;

        public CustomerUserInvitationSubmittedEventHandler(
            ILogger<CustomerUserInvitationSubmittedEventHandler> logger, TrangloUserManager userManager,
            INotificationService notificationService, IConfiguration config,
            IBusinessProfileRepository businessProfileRepository,
            IWebHostEnvironment environment, IPartnerRepository partnerRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _notificationService = notificationService;
            _config = config;
            _businessProfileRepository = businessProfileRepository;
            _environment = environment;
            _partnerRepository = partnerRepository;
        }

        protected override async Task HandleAsync(CustomerUserInvitationSubmittedEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Generating invitation email for {@event.Email}.");
            _logger.LogDebug($"New Customer User value: {@event.IsNewCustomerUser}.");
            var _Customer = await _userManager.FindByIdAsync(@event.Email);
            var _businessprofile = await _businessProfileRepository.GetBusinessProfileByCompanyNameAsync(@event.CompanyName);
            var _customerUserBusinessProfile = await _businessProfileRepository.GetCustomerUserBusinessProfileByBusinessProfileCodeAsync(@event.BusinessProfileCode);

            if (@event.BusinessProfileCode != null && _customerUserBusinessProfile != null)
            {
                var partnerRegistration = await _partnerRepository.GetPartnerRegistrationByBusinessProfileCodeAsync(_customerUserBusinessProfile.BusinessProfileCode);
                var partnerSubInfo = await _partnerRepository.GetPartnerSubscriptionListAsync(partnerRegistration.Id);
                bool isTrangloConnect = partnerSubInfo?.Any(x => x.Solution == Solution.Connect)??false;
                bool isTrangloBusiness = partnerSubInfo?.Any(x => x.Solution == Solution.Business)??false;

                string solution = string.Empty;

                if (isTrangloConnect && isTrangloBusiness)
                {
                    solution = "Tranglo Connect + Tranglo Business";
                }
                else if (isTrangloConnect)
                {
                    solution = "Tranglo Connect";
                }
                else if (isTrangloBusiness)
                {
                    solution = "Tranglo Business";
                }

                if (@event.IsNewCustomerUser)
                {
                    #region Send new user invitation email

                    // create a token    
                    string token = await _userManager.GenerateUserTokenAsync(_Customer, "InvitationDataProtectorTokenProvider", "Invitation");
                    string endcodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                    string url = _config.GetSection("IdentityServerUri").Value + "/account/InviteePasswordVerification/?email=" + _Customer.Email.Value + "&token=" + endcodedToken;

                    try
                    {
                        var recipients = new List<RecipientsInputDTO>
                        {
                            new RecipientsInputDTO()
                            {
                                email = _Customer.Email.Value,
                                name = _Customer.FullName.Value
                            }
                        };

                        var cc = new List<RecipientsInputDTO>();

                        var bcc = new List<RecipientsInputDTO>();

                        EmailNotificationInputDTO emailNotificationInputDTO = new EmailNotificationInputDTO();
                        emailNotificationInputDTO.recipients = recipients;
                        emailNotificationInputDTO.cc = cc;
                        emailNotificationInputDTO.bcc = bcc;
                        emailNotificationInputDTO.Url = url;
                        emailNotificationInputDTO.Entity = solution;
                        emailNotificationInputDTO.RecipientName = _Customer.FullName.Value;
                        emailNotificationInputDTO.CurrentUserName = @event.Inviter.FullName.Value;
                        emailNotificationInputDTO.UserId = _Customer.Email.Value;
                        emailNotificationInputDTO.InviterCompanyName = @event.CompanyName;
                        emailNotificationInputDTO.Token = endcodedToken;
                        emailNotificationInputDTO.NotificationTemplate = "InviteUserNewUserTemplate";
                        emailNotificationInputDTO.NotificationType = NotificationTypes.Email;
                        emailNotificationInputDTO.subject = $"({solution}) User invitation";
                        emailNotificationInputDTO.SolutionName = solution;

                        var sendInviteUserNewUserEmailResponse = await _notificationService.SendNotification(emailNotificationInputDTO);

                        if (sendInviteUserNewUserEmailResponse.IsFailure)
                        {
                            _logger.LogError("SendNotification", $"[CustomerUserInvitationSubmittedEventHandler] Invite New User Notification failed for {@event.CompanyName}. {sendInviteUserNewUserEmailResponse.Error}.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"[CustomerUserInvitationSubmittedEventHandler] {ex.ToString()}");
                    }
                    #endregion
                }
                else
                {
                    #region Send existing user invitation email

                    try
                    {
                        if (_Customer.AccountStatusCode == 1 || _Customer.AccountStatusCode == 6)
                        {
                            //Set CustomerUserBusinessProfile AccountStatus to Active  
                            var customerUserBusinessProfile = await _businessProfileRepository.GetCustomerUserBPById(_Customer.Id, _businessprofile.Id);

                            if (customerUserBusinessProfile.CompanyUserAccountStatus != CompanyUserAccountStatus.Active)
                            {
                                customerUserBusinessProfile.SetCompanyUserAccountStatus(CompanyUserAccountStatus.Active);
                                await _businessProfileRepository.UpdateCustomerUserBusinessProfileAsync(customerUserBusinessProfile, cancellationToken);
                            }
                        }

                        if (_Customer.AccountStatusCode == 2)
                        {
                            //Set CustomerUserBusinessProfile AccountStatus to Inactive  
                            var customerUserBusinessProfile = await _businessProfileRepository.GetCustomerUserBPById(_Customer.Id, _businessprofile.Id);

                            if (customerUserBusinessProfile.CompanyUserAccountStatus != CompanyUserAccountStatus.Inactive)
                            {
                                customerUserBusinessProfile.SetCompanyUserAccountStatus(CompanyUserAccountStatus.Inactive);
                                await _businessProfileRepository.UpdateCustomerUserBusinessProfileAsync(customerUserBusinessProfile, cancellationToken);
                            }
                        }

                        if (_Customer.AccountStatusCode == 5)
                        {
                            //Set CustomerUserBusinessProfile AccountStatus to PendingActivation  
                            var customerUserBusinessProfile = await _businessProfileRepository.GetCustomerUserBPById(_Customer.Id, _businessprofile.Id);

                            if (customerUserBusinessProfile.CompanyUserAccountStatus != CompanyUserAccountStatus.PendingActivation)
                            {
                                customerUserBusinessProfile.SetCompanyUserAccountStatus(CompanyUserAccountStatus.PendingActivation);
                                await _businessProfileRepository.UpdateCustomerUserBusinessProfileAsync(customerUserBusinessProfile, cancellationToken);
                            }
                        }

                        var recipients = new List<RecipientsInputDTO>
                        {
                            new RecipientsInputDTO()
                            {
                                email = _Customer.Email.Value,
                                name = _Customer.FullName.Value
                            }
                        };

                        var cc = new List<RecipientsInputDTO>();

                        var bcc = new List<RecipientsInputDTO>();

                        string url = _config.GetSection("IdentityServerUri").Value;

                        EmailNotificationInputDTO emailNotificationInputDTO = new EmailNotificationInputDTO();
                        emailNotificationInputDTO.recipients = recipients;
                        emailNotificationInputDTO.cc = cc;
                        emailNotificationInputDTO.bcc = bcc;
                        emailNotificationInputDTO.Url = url;
                        emailNotificationInputDTO.SolutionName = solution;
                        emailNotificationInputDTO.RecipientName = _Customer.FullName.Value; 
                        emailNotificationInputDTO.InviterCompanyName = @event.CompanyName;
                        emailNotificationInputDTO.NotificationTemplate = "InviteUserExistingUserTemplate";
                        emailNotificationInputDTO.NotificationType = NotificationTypes.Email;
                        emailNotificationInputDTO.subject = $"({solution}) User invitation";

                        var sendInviteUserExistingUserEmailResponse = await _notificationService.SendNotification(emailNotificationInputDTO);

                        if (sendInviteUserExistingUserEmailResponse.IsFailure)
                        {
                            _logger.LogError("SendNotification", $"[CustomerUserInvitationSubmittedEventHandler] Invite Existing User Notification failed for {@event.CompanyName}. {sendInviteUserExistingUserEmailResponse.Error}.");
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"[CustomerUserInvitationSubmittedEventHandler] {ex.ToString()}");
                    }
                    #endregion
                }
            }
            else
            {
                #region Send existing registered email

                try
                {
                    await _userManager.UpdateSecurityStampAsync(_Customer);

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
                    emailNotificationInputDTO.subject = "Sign up verification";

                    var sendSignUpVerificationEmailResponse = await _notificationService.SendNotification(emailNotificationInputDTO);

                    if (sendSignUpVerificationEmailResponse.IsFailure)
                    {
                        _logger.LogError("SendNotification", $"[CustomerUserInvitationSubmittedEventHandler] Sign Up Verification Notification failed for {@event.CompanyName}. {sendSignUpVerificationEmailResponse.Error}.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[CustomerUserInvitationSubmittedEventHandler] {ex.ToString()}");
                }
                #endregion
            }
        }
    }
}
