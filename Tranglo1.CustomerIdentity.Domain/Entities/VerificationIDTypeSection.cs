using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.Meta
{
    public class VerificationIDTypeSection : Entity<int>
    {
        public VerificationIDType VerificationIDType { get; set; }
        public string Description { get; set; }

        public VerificationIDTypeSection() : base ()
        {

        }

        public VerificationIDTypeSection(int id,string description) : base (id)
        {
            this.VerificationIDType = new VerificationIDType(id,description);
            Description = description;
        }
    }
}
