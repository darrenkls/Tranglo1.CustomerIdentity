using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerUser
{
    public class CompanyRoleInputDTO
    {
        public int CompanyCode { get; set; }
        public string UserRoleCode { get; set; }
        public int Action { get; set; }
    }
}
