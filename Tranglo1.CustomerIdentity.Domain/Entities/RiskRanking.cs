using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
   public class RiskRanking : Enumeration
    {
        public RiskRanking() : base()
        {

        }

        public RiskRanking(int id, string name) : base(id, name)
        {

        }

        public static readonly RiskRanking LOW = new RiskRanking(1, "Low");
        public static readonly RiskRanking MEDIUM = new RiskRanking(2, "Medium");
        public static readonly RiskRanking HIGH = new RiskRanking(3, "High");
      


    }
}

