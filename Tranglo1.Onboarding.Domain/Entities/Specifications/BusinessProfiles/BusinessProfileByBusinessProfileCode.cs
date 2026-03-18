using System;
using System.Linq.Expressions;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.Specifications.BusinessProfiles
{
    public sealed class BusinessProfileByBusinessProfileCode : Specification<BusinessProfile>
    {
        private readonly int _businessProfileCode;

        public BusinessProfileByBusinessProfileCode(int businessProfileCode)
        {
            _businessProfileCode = businessProfileCode;
        }

        public override Expression<Func<BusinessProfile, bool>> ToExpression()
        {
            return c => c.Id == _businessProfileCode;
        }
    }
}
