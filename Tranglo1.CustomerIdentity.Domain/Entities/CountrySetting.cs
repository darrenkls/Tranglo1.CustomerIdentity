using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class CountrySetting : Entity<int>
    {
        public CountryMeta Country { get; set; }
        public bool IsHighRisk { get; set; }
        public bool IsSanction { get; set; }
        public bool IsDisplay { get; set; }
        public bool IsRejectTransaction { get; set; }
    }        
}
