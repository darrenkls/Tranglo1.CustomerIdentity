using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class KYCCategoryCustomerType : Entity
    {
        public KYCCategory KYCCategory { get; set; }
        public int CustomerTypeGroupCode { get; set; }

        private KYCCategoryCustomerType() { }
    }
}