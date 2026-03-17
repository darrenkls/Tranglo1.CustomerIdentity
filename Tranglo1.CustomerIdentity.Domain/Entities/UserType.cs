using CSharpFunctionalExtensions;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class UserType : Enumeration
	{
        public UserType(): base()
        {
				
        }

		public UserType(int id, string name)
			: base(id, name)
		{

		}

		public static readonly UserType Undefined = new UserType(-1, "Undefined");
		public static readonly UserType Individual = new UserType(1, "Individual");
		public static readonly UserType Supplier = new UserType(2, "Supplier");
		public static readonly UserType Business = new UserType(3, "Business");

	}
}
