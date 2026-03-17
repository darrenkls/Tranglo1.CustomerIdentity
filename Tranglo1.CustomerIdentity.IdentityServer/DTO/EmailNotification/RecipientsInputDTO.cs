using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.EmailNotification
{
    public class RecipientsInputDTO
    {
        public string email { get; set; }
        public string name { get; set; }
        public string fullname { get; set; }
    }
}
