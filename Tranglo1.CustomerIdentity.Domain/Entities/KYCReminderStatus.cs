using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class KYCReminderStatus : Enumeration
    {

        private KYCReminderStatus() { }
        public KYCReminderStatus(int id, string name)
            : base(id, name)
        {

        }

        public static readonly KYCReminderStatus Unsubscribed = new KYCReminderStatus(1, "KYC Reminder Unsubscribed");
        public static readonly KYCReminderStatus Expired = new KYCReminderStatus(2, "KYC Reminder Expired");
    }
}
