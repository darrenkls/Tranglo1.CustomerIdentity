using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.Specifications.Category
{
    public sealed class CategoryBpInfoByCategoryCode : Specification<DocumentCategoryBP>
    {
        private readonly long _documentCategoryCode;

        public CategoryBpInfoByCategoryCode(long documentCategoryCode)
        {
            _documentCategoryCode = documentCategoryCode;
        }

        public override Expression<Func<DocumentCategoryBP, bool>> ToExpression()
        {
            return c => c.Id == _documentCategoryCode;
        }
    }
}
