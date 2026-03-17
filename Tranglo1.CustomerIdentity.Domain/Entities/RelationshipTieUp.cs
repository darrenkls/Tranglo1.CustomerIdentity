using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class RelationshipTieUp : Enumeration
    {
        public  RelationshipTieUp() : base()
        {

        }

        public RelationshipTieUp(int id,string name)
            :base (id,name)
        {

        }

        public static readonly RelationshipTieUp Sales_Partner_of_Tranglo = new RelationshipTieUp(1, "Sales Partner");
        public static readonly RelationshipTieUp Supply_Partner_of_Tranglo  = new RelationshipTieUp(2, "Supply Partner");
        public static readonly RelationshipTieUp Bilateral_of_Tranglo = new RelationshipTieUp(3, "Bilateral");
        public static readonly RelationshipTieUp Cryptocurrency_Exchange_of_Tranglo = new RelationshipTieUp(4, "Cryptocurrency Exchange of Tranglo");
    }
}
