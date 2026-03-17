using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.CountrySetting
{
    public class PostCountrySettingInputDTO
    {
        public string CountryISO2 { get; set; }
        public bool IsHighRisk { get; set; }
        public bool IsSanction { get; set; }
        public bool IsDisplay { get; set; }
        public bool IsRejectTransaction { get; set; }
    }
}
