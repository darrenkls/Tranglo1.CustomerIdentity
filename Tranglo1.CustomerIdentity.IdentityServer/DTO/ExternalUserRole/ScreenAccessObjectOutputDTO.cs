using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole
{
    public class ScreenAccessObjectOutputDTO
    {
        public string SubMenuCode { get; set; }
        public List<UACActionOutputDTO> UACActions { get; set; }
    }
}
