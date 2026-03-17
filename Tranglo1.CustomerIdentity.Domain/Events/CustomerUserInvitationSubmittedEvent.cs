using System;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.Domain.Events
{
    public class CustomerUserInvitationSubmittedEvent : DomainEvent
    {
		public ApplicationUser Inviter { get; private set; }
		public string Email { get; private set; }
		public string FullName { get; private set; }
		public DateTime RegisteredDate { get; private set; }
        public bool IsNewCustomerUser { get; set; }
        public string CompanyName { get; set; }
		public int? BusinessProfileCode { get; set; }

        public CustomerUserInvitationSubmittedEvent(ApplicationUser inviter, string email, string fullname, DateTime registeredDate, bool isNewCustomerUser, string companyName, int? businessProfileCode)
		{
			Inviter = inviter;
			Email = email;
			FullName = fullname;
			RegisteredDate = registeredDate;
			IsNewCustomerUser = isNewCustomerUser;
			CompanyName = companyName;
			BusinessProfileCode = businessProfileCode;
		}
	}
}
