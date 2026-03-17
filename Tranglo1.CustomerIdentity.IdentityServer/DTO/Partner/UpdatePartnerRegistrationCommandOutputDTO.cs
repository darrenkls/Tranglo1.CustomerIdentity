using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.Partner
{
    public class UpdatePartnerRegistrationCommandOutputDTO
    {
        public long PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string CustomerType { get; set; }
        public string FullRegisteredCompanyLegalName { get; set; }
        public string FormerRegisteredCompanyName { get; set; }
        public string TradeName { get; set; }
        public string CompanyRegisteredNo { get; set; }
        public ContactNumber ContactTelephone { get; set; }
        public string ContactPersonName {get;set;}
        public Email ContactEmailAddress { get; set; }
        public string CompanyPostCode { get; set; }
        public CountryMeta CompanyCountry { get; set; }
        public RelationshipTieUp RelationshipForTieUp { get; set; }
    }
}
