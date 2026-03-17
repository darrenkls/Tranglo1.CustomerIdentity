using System;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Events;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class Invitation : AggregateRoot
    {
        protected Invitation()
        {
        }

        public Invitation(ApplicationUser inviter, int? businessProfileCode, FullName fullName, Email email, bool isNewCustomerUser, string companyName )
		{
          
            InviterId = inviter.Id;
            BusinessProfileCode = businessProfileCode;
            SetName(fullName);
            SetEmail(email);

            //raise invitation submited event -> send email(invitation email)
            CustomerUserInvitationSubmittedEvent customerUserInvitationSubmittedEvent =
                        new CustomerUserInvitationSubmittedEvent(inviter, email.Value, fullName.Value, DateTime.Now, isNewCustomerUser, companyName, businessProfileCode);
            AddDomainEvent(customerUserInvitationSubmittedEvent);
        }

        public ApplicationUser Inviter { get; private set; }
        public int InviterId { get; set; }
        public BusinessProfile BusinessProfile { get; private set; }
        public int? BusinessProfileCode { get; set; }
        public FullName FullName { get; private set; }
        public Email Email { get; set; }

        public void SetName(FullName fullname)
        {
            FullName = fullname;
        }

        public virtual void SetEmail(Email email)
        {
            Email = email;
        }
    }
}
