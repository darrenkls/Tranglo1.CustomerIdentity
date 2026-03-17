using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerUser
{
    public class UpdatePartnerUserAdminInputDTO
    {
        public string Name { get; set; }
        public List<CompanyRoleInputDTO> CompanyRole { get; set; }
        public string Timezone { get; set; }
    }
}
