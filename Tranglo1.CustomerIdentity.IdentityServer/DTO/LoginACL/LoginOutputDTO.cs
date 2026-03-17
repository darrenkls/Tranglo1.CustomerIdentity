using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.LoginACL
{
    public class LoginOutputDTO
    {
        public DateTime UTCLoginDatetime { get; set; }
        public List<string> SubMenuPermissionCodes { get; set; }

        public LoginOutputDTO()
        {
            this.SubMenuPermissionCodes = new List<string>();
        }
    }
}
