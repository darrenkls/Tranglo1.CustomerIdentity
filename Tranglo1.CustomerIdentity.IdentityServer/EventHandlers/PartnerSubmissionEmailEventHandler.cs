using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.IdentityServer.Common.EventHandlers;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;
using System.IO;
using System.Text;
using System.Xml;
using Tranglo1.CustomerIdentity.Domain.Events;
using System;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.EmailNotification;
using System.Collections.Generic;
using Tranglo1.CustomerIdentity.Infrastructure.Repositories;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using System.Net;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.IdentityServer.EventHandlers
{
    class PartnerSubmissionEmailEventHandler : BaseEventHandler<PartnerSubmissionEmailEvent>
    {
        private readonly ILogger<PartnerSubmissionEmailEventHandler> _logger;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _config;
        private readonly IBusinessProfileRepository _businessProfileRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IApplicationUserRepository _applicationUserRepository;

        public PartnerSubmissionEmailEventHandler(
            ILogger<PartnerSubmissionEmailEventHandler> logger,
            INotificationService notificationService,
            IConfiguration config,
            IBusinessProfileRepository businessProfileRepository,
            IWebHostEnvironment environment,
            IApplicationUserRepository applicationUserRepository)
        {
            _logger = logger;
            _notificationService = notificationService;
            _config = config;
            _businessProfileRepository = businessProfileRepository;
            _environment = environment;
            _applicationUserRepository = applicationUserRepository;
        }
      
        protected override async Task HandleAsync(PartnerSubmissionEmailEvent @event, CancellationToken cancellationToken)
        {
            //var xsltTemplateRootPath = (string)AppDomain.CurrentDomain.GetData("ContentRootPath") + "\\wwwroot\\templates\\emailtemplate";
            var xsltTemplateRootPath = Path.Combine(_environment.ContentRootPath, "templates/emailtemplate");
            string cultureName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            var generator = new Common.ContentGenerator(xsltTemplateRootPath);
            var applicationUserInfo = await _applicationUserRepository.GetApplicationUserByUserId(@event.UserId);
            var businessProfileInfo = await _businessProfileRepository.GetBusinessProfileByCodeAsync(@event.BusinessProfileCode);

            // 2. Use xsl to inject the properties from @event
            StringBuilder _xml = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(_xml))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("PartnerSubmission");
                writer.WriteElementString("PICName", @event.PICName);
                writer.WriteElementString("CurrentYear", "&copy; " + DateTime.Today.Year.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            string content = generator.GenerateContent(_xml.ToString(), "PartnerSubmissionEmailTemplate", cultureName);

            var recipient = new List<RecipientsInputDTO>()
                            { new RecipientsInputDTO()
                                { email = applicationUserInfo.Email.Value, name = businessProfileInfo.CompanyName }
                            };

            Result<HttpStatusCode> sendBusinessPartnerGoLiveEmailResponse =
                            await _notificationService.SendNotification
                            (
                                recipient, null,
                                null, new List<IFormFile>() { },
                                "Your Application for Tranglo Business is Under Review",
                                content, NotificationTypes.Email
                            );

        }
    }
}
