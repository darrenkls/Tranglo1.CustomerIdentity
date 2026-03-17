using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerUser
{
    public class StagingUserPartnerDetailsInputDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int UserRoleCode { get; set; }
        public string Country { get; set; }
        public int UserEnvironmentCode { get; set; }
        public int AccountStatusCode { get; set; }
        public string TimezoneCode { get; set; }
    }
}
