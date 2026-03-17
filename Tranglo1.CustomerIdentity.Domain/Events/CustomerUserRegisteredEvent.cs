using System;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Events
{
    public class CustomerUserRegisteredEvent : DomainEvent
	{
		public string Email { get; private set; }
		public string FullName { get; private set; }
		public DateTime RegisteredDate { get; private set; }
		public int? SolutionCode { get; set; }
		public bool IsMultipleSolutions { get; set; }

		public CustomerUserRegisteredEvent(string email, string fullname, DateTime registeredDate, int? solutionCode, bool isMultipleSolutions)
		{
			this.Email = email;
			this.FullName = fullname;
			this.RegisteredDate = registeredDate;
			this.SolutionCode = solutionCode;
			this.IsMultipleSolutions = isMultipleSolutions;
		}
	}
}
