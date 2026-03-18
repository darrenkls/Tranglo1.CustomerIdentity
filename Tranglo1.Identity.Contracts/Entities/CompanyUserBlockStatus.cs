using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class CompanyUserBlockStatus : Enumeration
    {
        public CompanyUserBlockStatus() : base()
        {
        }

        public CompanyUserBlockStatus(int id, string name)
            : base(id, name)
        {

        }

        public static readonly CompanyUserBlockStatus Block = new CompanyUserBlockStatus(1, "Block");
        public static readonly CompanyUserBlockStatus Unblock = new CompanyUserBlockStatus(2, "Unblock");
    }
}
