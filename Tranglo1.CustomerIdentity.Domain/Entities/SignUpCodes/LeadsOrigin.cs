using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.SignUpCodes
{
    public class LeadsOrigin : Enumeration
	{
		public LeadsOrigin() : base()
		{

		}

		public LeadsOrigin(int id, string name)
			: base(id, name)
		{

		}


		public static readonly LeadsOrigin WordOfMouth = new LeadsOrigin(1, "Word Of Mouth");
		public static readonly LeadsOrigin SocialMedia = new LeadsOrigin(2, "Social Media");
		public static readonly LeadsOrigin OnlineAds = new LeadsOrigin(3, "Online Ads");
		public static readonly LeadsOrigin EmailMarketing = new LeadsOrigin(4, "Email Marketing");
		public static readonly LeadsOrigin WebsiteSeo = new LeadsOrigin(5, "Website / SEO");
		public static readonly LeadsOrigin News = new LeadsOrigin(6, "News");

	}
}
