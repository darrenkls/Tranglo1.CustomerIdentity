using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class ShareholderType : Enumeration
    {
        public ShareholderType() : base()
        {
        }

        public ShareholderType(int id, string name)
            : base(id, name)
        {

        }

        public static readonly ShareholderType Individual = new ShareholderType(1, "Individual"); //Default value 
        public static readonly ShareholderType Company = new ShareholderType(2, "Company");
    }
}
