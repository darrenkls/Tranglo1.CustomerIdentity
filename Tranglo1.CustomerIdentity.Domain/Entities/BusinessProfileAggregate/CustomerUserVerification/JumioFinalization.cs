using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.CustomerUserVerification
{
    public class JumioFinalization : Entity
    {
        public long CustomerVerificationDocumentCode { get; set; }
        public string Timestamp { get; set; }
        public string AccountId { get; set; }
        public string WorkflowExecutionId { get; set; }

        private JumioFinalization() { }
    }
}
