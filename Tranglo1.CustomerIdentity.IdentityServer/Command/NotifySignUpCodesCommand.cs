using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.EmailNotification;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.SignUpCode;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    public class NotifySignUpCodesCommand : BaseCommand<Result>
    {
        public long signUpCode { get; set; }
        public SignUpCodesNotificationInputDTO signUpCodesNotification;

        public override Task<string> GetAuditLogAsync(Result result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Sent sign-up code email";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
    }

    internal class NotifySignUpCodesCommandHandler : IRequestHandler<NotifySignUpCodesCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly ISignUpCodeRepository _signUpCodeRepository;
        private readonly ILogger<NotifySignUpCodesCommandHandler> _logger;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;
        private readonly IPartnerRepository _partnerRepository;

        public NotifySignUpCodesCommandHandler(IMapper mapper,
        IApplicationUserRepository applicationUserRepository,
        ISignUpCodeRepository signUpCodeRepository,
        IConfiguration config,
        INotificationService notificationService,
        ILogger<NotifySignUpCodesCommandHandler> logger, IWebHostEnvironment environment,
        IPartnerRepository partnerRepository)

        {
            _mapper = mapper;
            _logger = logger;
            _signUpCodeRepository = signUpCodeRepository;
            _applicationUserRepository = applicationUserRepository;
            _notificationService = notificationService;
            _config = config;
            _environment = environment;
            _partnerRepository = partnerRepository;
        }

        public async Task<Result> Handle(NotifySignUpCodesCommand request, CancellationToken cancellationToken)
        {
            
               // var xsltTemplateRootPath = (string)AppDomain.CurrentDomain.GetData("ContentRootPath") + "\\wwwroot\\templates\\emailtemplate";
                var xsltTemplateRootPath = Path.Combine(_environment.ContentRootPath, "templates/emailtemplate");
                string cultureName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                var generator = new Common.ContentGenerator(xsltTemplateRootPath);
                var signUpCodeInfo = await _signUpCodeRepository.GetSignUpCodesAsync(request.signUpCode);
                var partnerSubInfo = await _partnerRepository.GetPartnerSubscriptionListAsync(request.signUpCodesNotification.PartnerCode);
                bool isTrangloConnectExist = partnerSubInfo.Any(x => x.Solution == Solution.Connect);
                bool isTrangloBusinessExist = partnerSubInfo.Any(x => x.Solution == Solution.Business);
                string loginUrl = _config.GetValue<string>("IdentityServerUri") +"/Account/Register?signupcode=true";

            if (isTrangloConnectExist == true)
            {
                StringBuilder _xml = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(_xml))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Profile");
                    writer.WriteElementString("CompanyName", signUpCodeInfo.CompanyName);
                    writer.WriteElementString("GeneratedCode", signUpCodeInfo.Code);
                    writer.WriteElementString("LoginUrl", loginUrl);
                    writer.WriteElementString("CurrentYear", "&copy; " + DateTime.Today.Year.ToString());
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                string content = generator.GenerateContent(_xml.ToString(), "SignUpCodeTemplate", cultureName);

                var recipients = new List<RecipientsInputDTO>();
                var inputSignUpCodeNotification = request.signUpCodesNotification;

                {
                    var recipientlist = new RecipientsInputDTO()
                    {
                        email = inputSignUpCodeNotification.Email,
                        name = inputSignUpCodeNotification.CompanyName,
                    };
                    recipients.Add(recipientlist);
                }

                string subject = $"(Tranglo Connect) Tranglo 1.0 Sign Up Code";
                var cc = new List<RecipientsInputDTO>();
                var bcc = new List<RecipientsInputDTO>();
                var sendEmailResponse = await _notificationService.SendNotification(
                                                recipients, cc, bcc,
                                                new List<IFormFile>() { },
                                                subject, content,
                                                NotificationTypes.Email
                                            );
                
            }

            if (isTrangloBusinessExist == true)
            {
                StringBuilder _xml = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(_xml))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Profile");
                    writer.WriteElementString("CompanyName", signUpCodeInfo.CompanyName);
                    writer.WriteElementString("GeneratedCode", signUpCodeInfo.Code);
                    writer.WriteElementString("LoginUrl", loginUrl);
                    writer.WriteElementString("CurrentYear", "&copy; " + DateTime.Today.Year.ToString());
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                string content = generator.GenerateContent(_xml.ToString(), "SignUpCodeTemplate", cultureName);

                var recipients = new List<RecipientsInputDTO>();
                var inputSignUpCodeNotification = request.signUpCodesNotification;

                {
                    var recipientlist = new RecipientsInputDTO()
                    {
                        email = inputSignUpCodeNotification.Email,
                        name = inputSignUpCodeNotification.CompanyName,
                    };
                    recipients.Add(recipientlist);
                }

                string subject = $"(Tranglo Business) Tranglo 1.0 Sign Up Code";
                var cc = new List<RecipientsInputDTO>();
                var bcc = new List<RecipientsInputDTO>();
                var sendEmailResponse = await _notificationService.SendNotification(
                                                recipients, cc, bcc,
                                                new List<IFormFile>() { },
                                                subject, content,
                                                NotificationTypes.Email
                                            );

            }
            return Result.Success<string>("SignUpCode Emailed");

        }
    }
    }

