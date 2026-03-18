using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.CustomerUserVerification
{
    public class JumioVerification : Entity
    {
        public long CustomerVerificationDocumentCode { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string CustomerInternalReference { get; set; }

        private JumioVerification() { }
    }
}