using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.TransactionEvaluation
{
    public class TransactionEvaluationInfo: Entity
    {
        public string TransactionEvaluationInfoDescription { get; set; }
        public int CustomerTypeGroupCode { get; set; }
    }
}
