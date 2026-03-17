using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Microsoft.Extensions.Configuration;
using CSharpFunctionalExtensions;
using System.Xml;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.EmailNotification;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Tranglo1.CustomerIdentity.IdentityServer.CustomerUserList.Commands
{
    public class ResetPasswordCommand : IRequest<IdentityResult>
    {
        public string Email { get; set; }
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, IdentityResult>
    {
        private readonly TrangloUserManager _userManager;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _config;
        private readonly ILogger<ResetPasswordCommand> _logger;
        private readonly IWebHostEnvironment _environment;

        public ResetPasswordCommandHandler(
            TrangloUserManager userManager, INotificationService notificationService,
            IConfiguration config, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _notificationService = notificationService;
            _config = config;
            _environment = environment;
        }

        public async Task<IdentityResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            //Validate if email provided is in the correct email format
            var emailResult = Email.Create(request.Email);

			if (emailResult.IsFailure)
			{
                return IdentityResult.Failed(new IdentityError() { Description = emailResult.Error });
			}

            //find user by email provided
            var user  = await _userManager.FindByIdAsync(emailResult.Value.Value);

			if (user == null)
			{
                return IdentityResult.Failed(new IdentityError() { Description = $"Email: {emailResult.Value.Value} is not a valid customer user email." });
            }

            if (user is TrangloStaff)
			{
                //Artificial success, should not have someone trying to use Tranglo AD to reset
                //In case we get this kind of request, just ignore it.
                return IdentityResult.Success;
            }

			if (user.AccountStatus != AccountStatus.Active)
			{
                return IdentityResult.Failed(new IdentityError() { 
                    Description = $"User: {user.FullName.Value} account status is inactive. Unable to proceed on password reset." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var endcodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string url = _config.GetValue<string>("IdentityServerUri") + "/account/createpassword?email=" + emailResult.Value.Value + "&token=" + endcodedToken;
            //Result<string> emailTemplate = _notificationService.GetTemplate(NotificationTemplate.PasswordReset);
            //if (emailTemplate.IsFailure)
            //{
            //    //_logger.LogError("GetTemplate", $"User {emailResult.Value.Value} sent invitation email failed. {emailTemplate.Error}.");
            //    return IdentityResult.Failed(new IdentityError() { Description = $"User {emailResult.Value.Value} sent password reset email failed. {emailTemplate.Error}." });
            //}

            //string MailTextResetPassword = emailTemplate.Value;
            //MailTextResetPassword = MailTextResetPassword.Replace("[EmailServiceUri]", _config.GetValue<string>("IdentityServerUri"))
            //                                                    .Replace("[UserName]", user.FullName.Value)
            //                                                    .Replace("[Email]", emailResult.Value.Value)
            //                                                    .Replace("[Token]", endcodedToken)
            //                                                    .Replace("[CurrentYear]", DateTime.UtcNow.Year.ToString()
            //                                                    );

            //var sendEmailResponse = await _notificationService.SendNotification(
            //                                        emailResult.Value.Value, 
            //                                        "Tranglo 1.0 Reset Password",
            //                                        MailTextResetPassword, 
            //                                        user.FullName.Value,
            //                                        _config.GetValue<string>("IdentityServerUri"),
            //                                        NotificationTypes.Email
            //                                      );

            //if (sendEmailResponse.IsFailure)
            //{
            //    return IdentityResult.Failed(new IdentityError() { Description = $"User {emailResult.Value.Value} sent email failed. {sendEmailResponse.Error}." });
            //}
            
                StringBuilder _xml = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(_xml))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Profile");
                    writer.WriteElementString("EmailServiceUri", url);
                    writer.WriteElementString("UserName", user.FullName.Value);
                    writer.WriteElementString("Email", emailResult.Value.Value);
                    writer.WriteElementString("Token", endcodedToken);
                    writer.WriteElementString("CurrentYear", DateTime.UtcNow.Year.ToString());
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

            //var xsltTemplateRootPath = (string)AppDomain.CurrentDomain.GetData("ContentRootPath") + "\\wwwroot\\templates\\emailtemplate";
                var xsltTemplateRootPath = Path.Combine(_environment.ContentRootPath, "templates/emailtemplate");
                string cultureName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                var generator = new Common.ContentGenerator(xsltTemplateRootPath);
                string content = generator.GenerateContent(_xml.ToString(), "PasswordResetTemplate", cultureName);

                string MailTextDocumentReleased = content;

                var recipients = new List<RecipientsInputDTO>();

                var recipientlist = new RecipientsInputDTO()
                {
                    email = emailResult.Value.Value,
                    name = user.FullName.Value
                };
                recipients.Add(recipientlist);

                var sendInviteUserExistingUserEmailResponse = await _notificationService.SendNotification(

                                    recipients,
                                    null,
                                    null,
                                    null,
                                    "Tranglo 1.0 Reset Password",
                                    MailTextDocumentReleased,
                                    NotificationTypes.Email
                                );


                if (sendInviteUserExistingUserEmailResponse.IsFailure)
                {
                    return IdentityResult.Failed(new IdentityError() { Description = $"User {emailResult.Value.Value} sent email failed. {sendInviteUserExistingUserEmailResponse.Error}." });
                }

                return IdentityResult.Success;
            }
    }
}
