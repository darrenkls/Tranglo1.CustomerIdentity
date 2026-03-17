using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Events;

namespace Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement
{
    public class PartnerAccountStatus : Entity   
    {
        //public PartnerRegistration PartnerRegistration { get; set; }
        public ChangeType ChangeType { get; set; }
        public string Description { get; set; }
        public PartnerAccountStatusType PartnerAccountStatusType { get; set; }
        public long? PartnerSubscriptionCode { get; set; }

        private PartnerAccountStatus()
        {

        }
        public PartnerAccountStatus(ChangeType changeType, string description, 
            PartnerAccountStatusType partnerAccountStatusType, long partnerSubscriptionCode)
        {
            this.ChangeType = changeType;
            this.Description = description;
            this.PartnerAccountStatusType = partnerAccountStatusType;
            this.PartnerSubscriptionCode = partnerSubscriptionCode;
        }
    }

}
