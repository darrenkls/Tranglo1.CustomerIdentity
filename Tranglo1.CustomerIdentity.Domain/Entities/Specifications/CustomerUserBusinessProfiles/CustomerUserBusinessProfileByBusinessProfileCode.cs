using System;
using System.Linq.Expressions;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.Specifications.CustomerUserBusinessProfiles
{
    public sealed class CustomerUserBusinessProfileByBusinessProfileCode : Specification<CustomerUserBusinessProfile>
    {
        private readonly int _businessProfileCode;

        public CustomerUserBusinessProfileByBusinessProfileCode(int businessProfileCode)
        {
            _businessProfileCode = businessProfileCode;
        }

        public override Expression<Func<CustomerUserBusinessProfile, bool>> ToExpression()
        {
            return c => c.BusinessProfileCode == _businessProfileCode;
        }
    }
}
