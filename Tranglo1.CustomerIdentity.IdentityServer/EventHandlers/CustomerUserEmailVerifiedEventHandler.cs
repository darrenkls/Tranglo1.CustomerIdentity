using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Common.EventHandlers;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate;
using Tranglo1.CustomerIdentity.Domain.Entities.Specifications.BusinessProfiles;
using Tranglo1.CustomerIdentity.Domain.Entities.Specifications.CustomerUserBusinessProfileRoles;
using Tranglo1.CustomerIdentity.Domain.Entities.Specifications.CustomerUserBusinessProfiles;
using Tranglo1.CustomerIdentity.Domain.Events;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Domain.Common;
using System.Linq;
using System;
using System.Text;
using System.Xml;
using System.Net;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.EmailNotification;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement;

namespace Tranglo1.CustomerIdentity.IdentityServer.CustomerUserList.EventHandlers
{
    class CustomerUserEmailVerifiedEventHandler : BaseEventHandler<CustomerUserEmailVerifiedEvent> // INotificationHandler<DomainEventNotification<CustomerUserEmailVerifiedEvent>>
    {
        private readonly ILogger<CustomerUserEmailVerifiedEvent> _logger;
        private readonly TrangloUserManager userManager;
        private readonly BusinessProfileService businessProfileService;
        private readonly IExternalUserRoleRepository _iexternalUserRoleRepository;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _config;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IPartnerRepository _partnerRepository;

        public CustomerUserEmailVerifiedEventHandler(
            ILogger<CustomerUserEmailVerifiedEvent> logger,
            IConfiguration config,
            TrangloUserManager userManager, INotificationService notificationService,
            IApplicationUserRepository applicationUserRepository,
            BusinessProfileService businessProfileService,
            IExternalUserRoleRepository iexternalUserRoleRepository,
            IWebHostEnvironment environment,
            IPartnerRepository partnerRepository)
        {
            _logger = logger;
            _notificationService = notificationService;
            _applicationUserRepository = applicationUserRepository;
            _config = config;
            this.userManager = userManager;
            this.businessProfileService = businessProfileService;
            _iexternalUserRoleRepository = iexternalUserRoleRepository;
            _environment = environment;
            _partnerRepository = partnerRepository;
        }

        protected override async Task HandleAsync(CustomerUserEmailVerifiedEvent @event, CancellationToken cancellationToken)
        {
            var domainEvent = @event;
            if (domainEvent.IsMultipleSolution == true)
            {
                int[] solutions = { 1, 2 };

                foreach (var solution in solutions)
                {
                    var customerUser = await this.userManager
                    .FindByIdAsync(domainEvent.CustomerEmail) as CustomerUser;

                    if (customerUser == null)
                    {
                        this._logger.LogWarning($"Customer [{domainEvent.CustomerEmail}] not found.");
                        return;
                    }

                    //UserRole[] allUserROles = Enumeration.GetAll<UserRole>().ToArray();
                    var externalUserRoles = await _iexternalUserRoleRepository.GetSystemAdminExternalRoleAsync();

                    var _CurrentProfiles = await businessProfileService.GetBusinessProfileByEmailAsync(customerUser);
                    var _CurrentProfile = _CurrentProfiles.Value.FirstOrDefault();
                    bool firstRun = (_CurrentProfile == null);

                    var businessProfileResult = await businessProfileService.EnsureBusinessProfileAsync(
                                                                            customerUser,
                                                                            domainEvent.CustomerEmail,
                                                                            externalUserRoles
                                                                        );


                    _logger.LogInformation($"Business profile has been initialized for customer [{domainEvent.CustomerEmail}]");

                    if (businessProfileResult.IsSuccess)
                    {
                        var customerUserRegistration = await _applicationUserRepository.GetCustomerUserRegistrationsByLoginIdAsync
                           (domainEvent.CustomerEmail);
                        var partnerRegisterEmailSubject = "";
                        var partnerProfile = await _partnerRepository.GetPartnerRegistrationByBusinessProfileCodeAsync(businessProfileResult.Value.Id);
                        PartnerSubscription partnerSubProfile;
                        var entityName = "";
                        if (solution == Solution.Connect.Id)
                        {
                            partnerRegisterEmailSubject = $"(Tranglo Connect) {customerUserRegistration.CompanyName} has registered with Tranglo 1.0";
                        }
                        else if (solution == Solution.Business.Id)
                        {
                            partnerSubProfile = await _partnerRepository.GetPartnerSubcriptionByPartnerCodeAndSolutionCodeAsync(partnerProfile.Id, solution);
                            entityName = partnerSubProfile.TrangloEntity;
                            partnerRegisterEmailSubject = $"(Tranglo Business) {customerUserRegistration.CompanyName} has registered with Tranglo 1.0";
                        }

                        //var xsltTemplateRootPath = (string)AppDomain.CurrentDomain.GetData("ContentRootPath") + "\\wwwroot\\templates\\emailtemplate";
                        var xsltTemplateRootPath = Path.Combine(_environment.ContentRootPath, "templates/emailtemplate");
                        string cultureName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                        var generator = new Common.ContentGenerator(xsltTemplateRootPath);

                        StringBuilder _xml = new StringBuilder();
                        using (XmlWriter writer = XmlWriter.Create(_xml))
                        {
                            writer.WriteStartDocument();
                            writer.WriteStartElement("Profile");

                            if (@event.SolutionCode.Value == Solution.Business.Id)
                            {
                                writer.WriteElementString("TrangloEntity", "Entity : " + entityName);
                                writer.WriteElementString("CustomerType", "Customer Type : " + partnerProfile.CustomerType.Name);
                            }
                            writer.WriteElementString("EmailAddress", domainEvent.CustomerEmail);
                            writer.WriteElementString("LoginUrl", _config.GetValue<string>("IntranetUri"));
                            writer.WriteElementString("CompanyName", customerUserRegistration.CompanyName);
                            writer.WriteElementString("CurrentYear", "&copy; " + DateTime.Today.Year.ToString());
                            writer.WriteEndElement();
                            writer.WriteEndDocument();
                        }
                        string content = generator.GenerateContent(_xml.ToString(), "PartnerRegistrationTemplate", cultureName);

                        long recipientType = 1; long notificationTemplate = 24;
                        var recipientInfo = await businessProfileService.GetRecipientEmail(recipientType, notificationTemplate);

                        var recipients = new List<RecipientsInputDTO>();

                        foreach (var emailist in recipientInfo)
                        {
                            var recipientlist = new RecipientsInputDTO()
                            {
                                email = emailist.Email,
                                name = emailist.Name
                            };
                            recipients.Add(recipientlist);
                        }

                        long ccType = 2;
                        var ccInfo = await businessProfileService.GetRecipientEmail(ccType, notificationTemplate);

                        var cc = new List<RecipientsInputDTO>();

                        foreach (var emailist in ccInfo)
                        {
                            var cclist = new RecipientsInputDTO()
                            {
                                email = emailist.Email,
                                name = emailist.Name
                            };
                            cc.Add(cclist);
                        }

                        long bccType = 3;
                        var bccInfo = await businessProfileService.GetRecipientEmail(bccType, notificationTemplate);

                        var bcc = new List<RecipientsInputDTO>();

                        foreach (var emailist in bccInfo)
                        {
                            var bcclist = new RecipientsInputDTO()
                            {
                                email = emailist.Email,
                                name = emailist.Name
                            };
                            bcc.Add(bcclist);
                        }

                        Result<HttpStatusCode> sendPartnerRegistrationNotificationEmailResponse =
                            await _notificationService.SendNotification
                            (
                                recipients, bcc,
                                cc, new List<IFormFile>() { },
                                partnerRegisterEmailSubject,
                                content, NotificationTypes.Email
                            );

                        if (sendPartnerRegistrationNotificationEmailResponse.IsFailure)
                        {
                            _logger.LogError("SendNotification", $"Partner Registration Notification failed for {customerUserRegistration.CompanyName} . {sendPartnerRegistrationNotificationEmailResponse.Error}.");
                        }
                    }
                }
            }
            else
            {
                var customerUser = await this.userManager
                .FindByIdAsync(domainEvent.CustomerEmail) as CustomerUser;

                if (customerUser == null)
                {
                    this._logger.LogWarning($"Customer [{domainEvent.CustomerEmail}] not found.");
                    return;
                }

                //UserRole[] allUserROles = Enumeration.GetAll<UserRole>().ToArray();
                var externalUserRoles = await _iexternalUserRoleRepository.GetSystemAdminExternalRoleAsync();

                var _CurrentProfiles = await businessProfileService.GetBusinessProfileByEmailAsync(customerUser);
                var _CurrentProfile = _CurrentProfiles.Value.FirstOrDefault();
                bool firstRun = (_CurrentProfile == null);

                var businessProfileResult = await businessProfileService.EnsureBusinessProfileAsync(
                                                                    customerUser,
                                                                    domainEvent.CustomerEmail,
                                                                    externalUserRoles
                                                                );

                _logger.LogInformation($"Business profile has been initialized for customer [{domainEvent.CustomerEmail}]");

                if (firstRun && businessProfileResult.IsSuccess)
                {
                    var customerUserRegistration = await _applicationUserRepository.GetCustomerUserRegistrationsByLoginIdAsync
                       (domainEvent.CustomerEmail);
                    var partnerRegisterEmailSubject = "";
                    var partnerProfile = await _partnerRepository.GetPartnerRegistrationByBusinessProfileCodeAsync(businessProfileResult.Value.Id);
                    PartnerSubscription partnerSubProfile;
                    var entityName = "";


                    if (domainEvent.SolutionCode == Solution.Connect.Id)
                    {
                        partnerRegisterEmailSubject = $"(Tranglo Connect) {customerUserRegistration.CompanyName} has registered with Tranglo 1.0";

                    }
                    else if (domainEvent.SolutionCode == Solution.Business.Id)
                    {
                        partnerSubProfile = await _partnerRepository.GetPartnerSubcriptionByPartnerCodeAndSolutionCodeAsync(partnerProfile.Id, (int)domainEvent.SolutionCode);
                        entityName = partnerSubProfile.TrangloEntity;
                        partnerRegisterEmailSubject = $"(Tranglo Business) {customerUserRegistration.CompanyName} has registered with Tranglo 1.0";
                    }

                    //var xsltTemplateRootPath = (string)AppDomain.CurrentDomain.GetData("ContentRootPath") + "\\wwwroot\\templates\\emailtemplate";
                    var xsltTemplateRootPath = Path.Combine(_environment.ContentRootPath, "templates/emailtemplate");
                    string cultureName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                    var generator = new Common.ContentGenerator(xsltTemplateRootPath);

                    StringBuilder _xml = new StringBuilder();
                    using (XmlWriter writer = XmlWriter.Create(_xml))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Profile");
                        if (@event.SolutionCode.Value == Solution.Business.Id)
                        {
                            writer.WriteElementString("TrangloEntity", "Entity : " + entityName);
                            writer.WriteElementString("CustomerType", "Customer Type : " + partnerProfile.CustomerType.Name);
                        }
                        writer.WriteElementString("EmailAddress", domainEvent.CustomerEmail);
                        writer.WriteElementString("LoginUrl", _config.GetValue<string>("IntranetUri"));
                        writer.WriteElementString("CompanyName", customerUserRegistration.CompanyName);
                        writer.WriteElementString("CurrentYear", "&copy; " + DateTime.Today.Year.ToString());
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                    string content = generator.GenerateContent(_xml.ToString(), "PartnerRegistrationTemplate", cultureName);

                    long recipientType = 1; long notificationTemplate = 24;
                    var recipientInfo = await businessProfileService.GetRecipientEmail(recipientType, notificationTemplate);

                    var recipients = new List<RecipientsInputDTO>();

                    foreach (var emailist in recipientInfo)
                    {
                        var recipientlist = new RecipientsInputDTO()
                        {
                            email = emailist.Email,
                            name = emailist.Name
                        };
                        recipients.Add(recipientlist);
                    }

                    long ccType = 2;
                    var ccInfo = await businessProfileService.GetRecipientEmail(ccType, notificationTemplate);

                    var cc = new List<RecipientsInputDTO>();

                    foreach (var emailist in ccInfo)
                    {
                        var cclist = new RecipientsInputDTO()
                        {
                            email = emailist.Email,
                            name = emailist.Name
                        };
                        cc.Add(cclist);
                    }

                    long bccType = 3;
                    var bccInfo = await businessProfileService.GetRecipientEmail(bccType, notificationTemplate);

                    var bcc = new List<RecipientsInputDTO>();

                    foreach (var emailist in bccInfo)
                    {
                        var bcclist = new RecipientsInputDTO()
                        {
                            email = emailist.Email,
                            name = emailist.Name
                        };
                        bcc.Add(bcclist);
                    }

                    Result<HttpStatusCode> sendPartnerRegistrationNotificationEmailResponse =
                        await _notificationService.SendNotification
                        (
                            recipients, bcc,
                            cc, new List<IFormFile>() { },
                            partnerRegisterEmailSubject,
                            content, NotificationTypes.Email
                        );

                    if (sendPartnerRegistrationNotificationEmailResponse.IsFailure)
                    {
                        _logger.LogError("SendNotification", $"Partner Registration Notification failed for {customerUserRegistration.CompanyName} . {sendPartnerRegistrationNotificationEmailResponse.Error}.");
                    }
                }
            }


        }
    }
}
