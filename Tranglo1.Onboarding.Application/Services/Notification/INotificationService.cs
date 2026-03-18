using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.EmailNotification;

namespace Tranglo1.CustomerIdentity.IdentityServer.Services.Notification
{
	public interface INotificationService
	{
		Task<Result<HttpStatusCode>> SendNotification(string to, string subject, string body, string userName, string redirectUri, NotificationTypes notificationTypes, NotificationTemplate? notificationTemplates = null);
		//Result<string> GetTemplate(NotificationTemplate request);

		Task<Result<HttpStatusCode>> SendNotification(List<RecipientsInputDTO> recipients, List<RecipientsInputDTO> bcc, List<RecipientsInputDTO> cc, List<IFormFile> attachments, string subject, string body, NotificationTypes notificationTypes, NotificationTemplate notificationTemplate = null);
		Task<Result<HttpStatusCode>> SendNotification(EmailNotificationInputDTO emailNotificationInputDTO);

    }
}