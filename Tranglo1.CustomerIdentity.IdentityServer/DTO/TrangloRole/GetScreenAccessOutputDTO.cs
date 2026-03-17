using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.TrangloRole
{
    public class GetScreenAccessOutputDTO
    {
        public string SubMenuCode { get; set; }
        public List<GetSubMenu> UacActions { get; set; }
    }
    public class GetSubMenu
    {
     public string SubMenuCode { get; set; }
    public string SubMenuPermissionCode { get; set; }
    public int UacActionCode { get; set; }
    }
}
