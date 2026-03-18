using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.RBAAggregate
{
    public class OverridingRulesParameter : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
