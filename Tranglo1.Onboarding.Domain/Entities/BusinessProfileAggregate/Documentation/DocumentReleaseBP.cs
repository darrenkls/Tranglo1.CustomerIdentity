using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate
{
    public class DocumentReleaseBP : Entity
    {
        public BusinessProfile BusinessProfile { get; set; }
        public Guid DocumentId { get; set; }
        public bool IsReleased { get; set; }
    }
}
