using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.Partner
{
    public class PartnerAPISettingsOutputDTO
    {
        public long PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public List<APIEnvironmentSettingsOutputDTO> APIEnvironmentSetting { get; set; }
    }
}
