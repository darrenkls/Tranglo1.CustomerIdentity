using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.SignUpCodes
{
    public class SignUpCode : AggregateRoot<long>
    {
        public LeadsOrigin LeadsOrigin { get; set; }
        public SignUpAccountStatus Status { get; set; }
        public long PartnerCode { get; set; }
        public string CompanyName { get; set; }
        public string Code { get; set; }
        public string AgentLoginId { get; set; }
        public long SolutionCode { get; set; }
        public DateTime ExpireAt { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
