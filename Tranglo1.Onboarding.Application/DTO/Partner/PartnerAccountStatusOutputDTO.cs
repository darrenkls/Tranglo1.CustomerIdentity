using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.Partner
{
    public class PartnerAccountStatusOutputDTO
    {
        public string ChangeStatus { get; set; }
        public string ChangeType { get; set; }
        public string Description { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}
