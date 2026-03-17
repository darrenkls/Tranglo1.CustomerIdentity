using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.EmailNotification;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Shareholder;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    internal class RequestResetPasswordCommand : BaseCommand<Result<IdentityResult>>
    {
        public string Email { get; set; }
        public static DateTime UtcNow { get; }

        public override Task<string> GetAuditLogAsync(Result<IdentityResult> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Partner Request Reset Password : [{this.Email}]";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
    }

    internal class RequestResetPasswordCommandHandler :
        IRequestHandler<RequestResetPasswordCommand, Result<IdentityResult>>
    {
        private readonly TrangloUserManager trangloUserManager;
        private readonly INotificationService notificationService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<IdentityResult> _logger;
        private readonly IBusinessProfileRepository _businessProfileRepository;
        private readonly IPartnerRepository _partnerRepository;

        public RequestResetPasswordCommandHandler(
            TrangloUserManager trangloUserManager,
            INotificationService notificationService,
            IConfiguration config, IWebHostEnvironment environment,
            ILogger<IdentityResult> logger,
            IBusinessProfileRepository businessProfileRepository,
            IPartnerRepository partnerRepository)
        {
            this.trangloUserManager = trangloUserManager;
            this.notificationService = notificationService;
            _config = config;
            _environment = environment;
            _logger = logger;
            _businessProfileRepository = businessProfileRepository;
            _partnerRepository = partnerRepository;
        }


        public async Task<Result<IdentityResult>> Handle(RequestResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await trangloUserManager.FindByIdAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<IdentityResult>("User is not exist.");

            }
            var customerUserBP = await _businessProfileRepository.GetCustomerUserBusinessProfilesByUserIdAsync(user.Id);
            var businessProfiles = await _businessProfileRepository.GetBusinessProfileByCodeAsync(customerUserBP.BusinessProfileCode);
            var partner = await _partnerRepository.GetPartnerRegistrationByBusinessProfileCodeAsync(customerUserBP.BusinessProfileCode);
            var partnerSubInfo = await _partnerRepository.GetPartnerSubscriptionByPartnerCodeAsync(partner.Id);
            Solution? solution = null;

            if (partnerSubInfo.Solution.Id == Solution.Connect.Id)
                solution = Solution.Connect;

            if (partnerSubInfo.Solution.Id == Solution.Business.Id)
                solution = Solution.Business;

            if (user == null || user is CustomerUser customerUser == false)
            {
                return Result.Failure<IdentityResult>("Unable to process password reset request.");
                //            return IdentityResult.Failed(
                //                new IdentityError() { Description = "Unable to process password reset request." });
            }
            else if (user.AccountStatusCode == AccountStatus.PendingActivation.Id)
            {
                return Result.Failure<IdentityResult>("Unable to process password reset request, email is not verified");
                //return IdentityResult.Failed(
                //    new IdentityError() { Description = "Unable to process password reset request, email is not verified" });
            }
            else if (user.AccountStatusCode == AccountStatus.Active.Id)
            {

                _logger.LogInformation("Partner Request Reset Password :");
                _logger.LogInformation($"Business Profile Code = {businessProfiles.Id}");
                _logger.LogInformation($"Email = {request.Email}");
                _logger.LogInformation($"Partner Name = {businessProfiles.CompanyName}");
                _logger.LogInformation($"Date Request = {DateTime.UtcNow}");

            }








            var passwordResetToken = await trangloUserManager.GeneratePasswordResetTokenAsync(user);
            var endcodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(passwordResetToken));
            string url = _config.GetValue<string>("IdentityServerUri") + "/account/createpassword?email=" + request.Email + "&token=" + endcodedToken;

            //Result<string> emailTemplate = notificationService.GetTemplate(NotificationTemplate.PasswordReset);
            //if (emailTemplate.IsFailure)
            //{
            //	//_logger.LogError("GetTemplate", $"User {emailResult.Value.Value} sent invitation email failed. {emailTemplate.Error}.");
            //	return IdentityResult.Failed(new IdentityError() { Description = $"User {request.Email} sent password reset email failed. {emailTemplate.Error}." });
            //}

            StringBuilder _xml = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(_xml))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Profile");
                writer.WriteElementString("EmailServiceUri", url);
                writer.WriteElementString("UserName", user.FullName.Value);
                writer.WriteElementString("Email", request.Email);
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
                email = request.Email,
                name = user.FullName.Value
            };
            recipients.Add(recipientlist);

            var sendInviteUserExistingUserEmailResponse = await notificationService.SendNotification(

                                recipients,
                                null,
                                null,
                                null,
                                $"({solution.Name}) Tranglo 1.0 Reset Password",
                                MailTextDocumentReleased,
                                NotificationTypes.Email
                            );



            if (sendInviteUserExistingUserEmailResponse.IsSuccess)
            {
                //return IdentityResult.Success;
                return Result.Success<IdentityResult>(IdentityResult.Success);
            }

            return Result.Failure<IdentityResult>("Unable to send password reset token.");
            //return IdentityResult.Failed(
            //		new IdentityError() { Description = "Unable to send password reset token." });
        }
    }
}
