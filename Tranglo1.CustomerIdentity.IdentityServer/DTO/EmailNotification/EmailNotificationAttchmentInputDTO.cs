using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.EmailNotification
{
    public class EmailNotificationAttchmentInputDTO
    {
        public Guid RequestId { get; set; }

        public List<IFormFile> attachments { get; set; }
    }
}
