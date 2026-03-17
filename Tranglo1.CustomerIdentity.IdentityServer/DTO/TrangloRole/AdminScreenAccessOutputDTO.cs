using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.Helper.ACL;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.TrangloRole
{
    public class AdminScreenAccessOutputDTO
    {
        public List<ScreenAccessMenu> ScreenAccessMenuList { get; set; }
    }

    
}
