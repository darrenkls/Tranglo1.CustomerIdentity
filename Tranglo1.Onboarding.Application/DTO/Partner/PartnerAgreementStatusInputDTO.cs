using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Common.ModelBinder;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.Partner
{
    public class PartnerAgreementStatusInputDTO
    {
        public DateTime AgreementStartDate { get; set; }
        [EndDateBinder]
        public DateTime AgreementEndDate { get; set; }
        public int AgreementStatus { get; set; }
    }
}
