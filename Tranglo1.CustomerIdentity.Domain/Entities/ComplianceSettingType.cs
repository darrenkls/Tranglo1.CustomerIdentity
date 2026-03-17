using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class ComplianceSettingType : Enumeration
	{

		public ComplianceSettingType() : base()
		{

		}

		public ComplianceSettingType(int id, string name)
			: base(id, name)
		{

		}

		// RBA
		public static readonly ComplianceSettingType Sender_Compliance_Limit_Setting = new ComplianceSettingType(1, "Sender Compliance Limit Setting");
		public static readonly ComplianceSettingType RBA = new ComplianceSettingType(2, "RBA");
        public static readonly ComplianceSettingType Compliance_Grouping_Setting = new ComplianceSettingType(3, "Compliance Grouping Setting");
        public static readonly ComplianceSettingType Beneficiary_Compliance_Limit_Setting = new ComplianceSettingType(4, "Beneficiary Compliance Limit Setting");
        public static readonly ComplianceSettingType Manage_Blacklist_Whitelist = new ComplianceSettingType(5, "Manage Blacklist/Whitelist");
    }
}
