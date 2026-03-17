using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.Documentation
{
    public class BusinessDocumentGroupCategory : Entity
    {
        public string GroupCategoryDescription { get; set; }
       
        public string TooltipDescription { get; set; }

        private BusinessDocumentGroupCategory()
        { 

        }
    }
}
