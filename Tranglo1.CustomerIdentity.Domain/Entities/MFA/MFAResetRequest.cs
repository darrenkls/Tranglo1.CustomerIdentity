using CSharpFunctionalExtensions;
using System;

namespace Tranglo1.CustomerIdentity.Domain.Entities.MFA
{
    public class MFAResetRequest : Entity
    {
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Token { get; set; }
        public bool IsUsed { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
