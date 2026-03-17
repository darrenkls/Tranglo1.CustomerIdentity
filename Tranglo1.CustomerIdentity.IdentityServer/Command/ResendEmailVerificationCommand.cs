using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
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
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.EmailNotification;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    public class ResendEmailVerificationCommand : BaseCommand<Result>

    {
        public string Email { get; set; }

        public class ResendEmailVerificationCommandHandler : IRequestHandler<ResendEmailVerificationCommand, Result>
        {
            private readonly ILogger<ResendEmailVerificationCommandHandler> _logger;
            private readonly TrangloUserManager userManager;
            private readonly INotificationService notificationService;
            private readonly IConfiguration _config;
            private readonly IWebHostEnvironment _environment;
            private readonly IBusinessProfileRepository _businessProfileRepository;
            private readonly IPartnerRepository _partnerRepository;

            public ResendEmailVerificationCommandHandler(
            ILogger<ResendEmailVerificationCommandHandler> logger, TrangloUserManager userManager,
            INotificationService notificationService, IConfiguration config,
            IWebHostEnvironment environment, IBusinessProfileRepository businessProfileRepository, IPartnerRepository partnerRepository)
            {
                _logger = logger;
                this.userManager = userManager;
                this.notificationService = notificationService;
                _config = config;
                _environment = environment;
                _businessProfileRepository = businessProfileRepository;
                _partnerRepository = partnerRepository;
            }

            public async Task<Result> Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken)
            {

                var _Customer = await this.userManager.FindByIdAsync(request.Email);

                if (_Customer != null)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(_Customer);

                    var userToken = await _businessProfileRepository.FindUserTokenByEmailAsync(request.Email);

                    if (userToken == null)
                    {
                        UserVerificationToken userVerificationToken = new UserVerificationToken()
                        {
                            Email = request.Email,
                            Token = token
                        };
                        await _businessProfileRepository.AddUserVerificationToken(userVerificationToken);

                    }
                    else
                    {
                        UserVerificationToken updateUserVerificationToken = new UserVerificationToken()
                        {
                            Email = request.Email,
                            Token = token
                        };
                        await _businessProfileRepository.UpdateUserVerificationToken(updateUserVerificationToken);
                    }

                    var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                    string confirmationUrl = $"{_config.GetValue<string>("IdentityServerUri")}/account/confirmemail/?userid={request.Email}&Token={code}";
                    string confirmationPlaceHolder = Guid.NewGuid().ToString();

                    // 1. get the email template
                    var xsltTemplateRootPath = Path.Combine(_environment.ContentRootPath, "templates/emailtemplate");
                    string cultureName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                    var generator = new Common.ContentGenerator(xsltTemplateRootPath);

                    // 2. Use xsl to inject the properties from @event
                    StringBuilder _xml = new StringBuilder();
                    using (XmlWriter writer = XmlWriter.Create(_xml))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Profile");
                        writer.WriteElementString("Name", _Customer.FullName.Value);
                        writer.WriteElementString("ConfirmationUrl", confirmationPlaceHolder);
                        writer.WriteElementString("CurrentYear", "&copy; " + DateTime.Today.Year.ToString());
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                    string content = generator.GenerateContent(_xml.ToString(), "VerifyEmailTemplate", cultureName);
                    content = content.Replace(confirmationPlaceHolder, confirmationUrl);

                    var recipient = new List<RecipientsInputDTO>(){ new RecipientsInputDTO()
                    { email = request.Email, name =_Customer.FullName.Value}
                    };

                    var cc = new List<RecipientsInputDTO>();

                    var bcc = new List<RecipientsInputDTO>();

                    var sendEmailResponse = await notificationService.SendNotification(
                                                       recipient,
                                                       bcc,
                                                       cc,
                                                       new List<IFormFile>() { },
                                                       "Sign up verification",
                                                       content,
                                                       NotificationTypes.Email
                                                    );
                }
                else
                {

                    return Result.Failure(
                                          $"Unable to resend email verification for email {request.Email}."
                                         );
                }
                return Result.Success(_Customer);
            }
        }
    }
}