using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.Specifications.Category
{
    public sealed class DocumentIdByCategoryBPCode : Specification<DocumentUploadBP>
    {
        private readonly long _documentCategoryBPCode;

        public DocumentIdByCategoryBPCode(long documentCategoryBPCode)
        {
            _documentCategoryBPCode = documentCategoryBPCode;
        }

        public override Expression<Func<DocumentUploadBP, bool>> ToExpression()
        {
            return c => c.DocumentCategoryBPCode == _documentCategoryBPCode;
        }
    }
}
