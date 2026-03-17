using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerUser
{
    public class ViewPartnerUserCustomerOutputDTO
    {
        public int BusinessProfileCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<UserRoles> UserRole { get; set; } = new List<UserRoles>();
        public int UserEnvironmentCode { get; set; }
        public string UserEnvironment { get; set; }
        public int AccountStatusCode { get; set; }
        public string AccountStatus { get; set; }
        public string Timezone { get; set; }
        public long? CompanyUserAccountStatusCode { get; set; }
        public string CompanyUserAccountStatus { get; set; }
        public long? CompanyUserBlockStatusCode { get; set; }
        public string CompanyUserBlockStatus { get; set; }

        public class UserRoles
        {
            public string UserRoleCode { get; set; }
            public string UserRole { get; set; }
        }
    }
}
