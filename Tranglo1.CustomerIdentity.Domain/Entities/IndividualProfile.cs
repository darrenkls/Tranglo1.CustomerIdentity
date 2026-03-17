using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class IndividualProfile: Entity
    {
        public CustomerUser CustomerUser { get; set; }
        public int UserId { get; set; }
        public Solution Solution { get; set; }
        public long SolutionCode { get; set; }

        //20210118: To Add more properties/fields once Individual Profile requirement is complete. Current focus is more towards Busuiness Profile
    }
}
