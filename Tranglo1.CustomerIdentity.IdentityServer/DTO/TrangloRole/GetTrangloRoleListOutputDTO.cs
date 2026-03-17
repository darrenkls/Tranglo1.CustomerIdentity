using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetTrangloRoleListOutputDTO
    {
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public string AuthorityLevelDescription { get; set; }
        public string DepartmentDescription { get; set; }
        public bool IsSuperApprover { get; set; }
        public int RoleStatusCode { get; set; }
        public string RoleStatusDescription { get; set; }

    }
}
