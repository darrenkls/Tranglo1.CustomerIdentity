using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerUser
{
    public class UpdatePartnerUserCustomerInputDTO
    {
        public string Name { get; set; }
        public List<UserRolesInputDTO> UserRole { get; set; }
        public string Timezone { get; set; }
    }
}
