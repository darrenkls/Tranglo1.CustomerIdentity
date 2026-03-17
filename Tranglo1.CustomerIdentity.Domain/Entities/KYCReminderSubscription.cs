using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class KYCReminderSubscription : Enumeration
    {
        public KYCReminderSubscription() : base()
        {

        }
        public KYCReminderSubscription(int id, string name)
            : base(id, name)
        {

        }
        public static readonly KYCReminderSubscription Subscribed = new KYCReminderSubscription(1, "Subscribe");
        public static readonly KYCReminderSubscription Unsubscribed = new KYCReminderSubscription(2, "Unsubscribe");
    }
}
