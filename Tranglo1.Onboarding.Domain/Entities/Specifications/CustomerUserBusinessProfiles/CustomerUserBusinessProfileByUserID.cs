using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.Specifications.CustomerUserBusinessProfiles
{
    public sealed class CustomerUserBusinessProfileByUserID : Specification<CustomerUserBusinessProfile>
    {
        private readonly int _userId;

        public CustomerUserBusinessProfileByUserID(int userId)
        {
            _userId = userId;
        }

        public override Expression<Func<CustomerUserBusinessProfile, bool>> ToExpression()
        {
            return c => c.UserId == _userId;
        }
    }
}
