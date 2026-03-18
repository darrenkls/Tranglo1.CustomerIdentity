using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement
{
    public class APIURL : Entity
    {
        public int Environment { get; set; }
        public string StringDomain { get; set; }
        public int APIType { get; set; }
    }
}
