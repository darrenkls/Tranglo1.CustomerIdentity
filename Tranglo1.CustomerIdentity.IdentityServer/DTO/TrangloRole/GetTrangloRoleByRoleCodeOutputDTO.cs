using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.TrangloRole;
using Tranglo1.CustomerIdentity.IdentityServer.Helper.ACL;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetTrangloRoleByRoleCodeOutputDTO
    {
        public string RoleName { get; set; }
        public int AuthorityLevelCode { get; set; }
        public int DepartmentCode{ get; set; }
        public bool IsSuperApprover { get; set; }

        public List<ScreenAccessMenu> ScreenAccessMenuList { get; set; }
    }
}
