using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.SignUpCode
{
    public class GetCountrySettingInputDTO
    {
        public string? CountryISO2 { get; set; }
        public bool? IsHighRisk { get; set; }
        public bool? IsSanction { get; set; }
        public bool? IsDisplay { get; set; }
        public bool? IsRejectTransaction { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
