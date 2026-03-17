using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Data
{
    internal class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Debug.WriteLine("============================SENDING EMAIL=====================");
            Debug.WriteLine($"Recipient : {email}");
            Debug.WriteLine($"Subject : {subject}");
            Debug.WriteLine($"Content : {htmlMessage}");

            return Task.CompletedTask;
        }
    }
}
