using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Helper.ACL;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole
{
    public class UpdateExternalUserRoleInputDTO
    {
        public List<ExternalPermissionDetail> PermissionInfoList { get; set; }
    }

    public class ExternalPermissionDetail
    {
        public string PermissionInfoCode { get; set; }
        public bool IsSelected { get; set; }
    }
}
