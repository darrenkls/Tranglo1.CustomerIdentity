using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.Meta
{
    public class RiskType : Enumeration
    {
        public RiskType() : base()
        {

        }

        public RiskType(int id,string name) : base(id, name)
        {

        }

        public static readonly RiskType NOT_EXECUTED = new RiskType(1, "NOT_EXECUTED");
        public static readonly RiskType PASSED = new RiskType(2, "PASSED");
        public static readonly RiskType REJECTED = new RiskType(3, "REJECTED");
        public static readonly RiskType WARNING = new RiskType(4, "WARNING");


    }
}
