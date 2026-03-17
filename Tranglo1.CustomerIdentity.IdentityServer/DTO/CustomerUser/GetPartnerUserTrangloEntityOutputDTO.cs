using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerUser
{
    public class GetPartnerUserTrangloEntityOutputDTO
    {
        public long? PartnerCode { get; set; }
        public long? SolutionCode { get; set; }
        public string SolutionDescription { get; set; }
        public string TrangloEntity{ get; set; }
    }
}
