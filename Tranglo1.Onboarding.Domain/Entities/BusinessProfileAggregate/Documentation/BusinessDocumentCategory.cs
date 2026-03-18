using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.Documentation
{
    public class BusinessDocumentCategory : Entity
    {
        public BusinessDocumentGroupCategory BusinessDocumentGroupCategory { get; set; }
        public DocumentCategoryGroup DocumentCategoryGroup { get; set; }
        public DocumentCategory DocumentCategory { get; set; }


        private BusinessDocumentCategory()
        {
        }

    }
}
