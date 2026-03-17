using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerUser
{
    public class UpdatePartnerUserCustomerOutputDTO
    {
        public string Name { get; set; }
        public List<UserRolesInputDTO> UserRole { get; set; }
        public string Timezone { get; set; }
    }
}

