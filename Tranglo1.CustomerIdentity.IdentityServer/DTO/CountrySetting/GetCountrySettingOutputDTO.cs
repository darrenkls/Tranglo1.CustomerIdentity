using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.SignUpCode
{
    public class GetCountrySettingOutputDTO
    {
        public int CountrySettingCode { get; set; }
        public string CountryISO2 { get; set; }
        public string CountryDescription { get; set; }
        public bool IsHighRisk { get; set; }
        public bool IsSanction { get; set; }
        public bool IsDisplay { get; set; }
        public bool IsRejectTransaction { get; set; }
        public string UpdatedBy{ get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
