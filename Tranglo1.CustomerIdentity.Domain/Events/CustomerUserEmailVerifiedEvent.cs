using System;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.Domain.Events
{
    public class CustomerUserEmailVerifiedEvent : DomainEvent
	{
        public string CustomerEmail { get; private set; }
		public int? SolutionCode { get; set; }
        public bool IsMultipleSolution { get; set; } 
	
        public CustomerUserEmailVerifiedEvent(string email, int? solutionCode, bool isMultipleSolution)
        {
            this.CustomerEmail = email;
            this.SolutionCode = solutionCode;
            this.IsMultipleSolution = isMultipleSolution;
        }
    }
}
