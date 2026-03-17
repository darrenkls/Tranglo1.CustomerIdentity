using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class TrangloDepartment : Enumeration
    {
        public string DepartmentPrefix { get; set; }
        public TrangloDepartment() : base()
        {

        }
        public TrangloDepartment(int id, string name, string departmentPrefix)
            : base(id, name)
        {
            this.DepartmentPrefix = departmentPrefix;
        }
        public static IEnumerable<TrangloDepartment> GetAllTrangloDepartment()
        {
            return _CachedEnumerations.GetOrAdd(typeof(TrangloDepartment), t =>
            {
                var fields = typeof(TrangloDepartment).GetFields(BindingFlags.Public |
                                                             BindingFlags.Static |
                                                             BindingFlags.DeclaredOnly |
                                                             BindingFlags.Instance);

                return fields
                    .Where(f => f.DeclaringType == t)
                    .Select(f => f.GetValue(null))
                    .Cast<TrangloDepartment>()
                    .OrderBy(e => e.Name)
                    .ToArray();

            }).Cast<TrangloDepartment>();
        }

        public static TrangloDepartment GetDepartmentByPrefix(string deptPrefix)
        {
            var deptPrefixValue = deptPrefix?.ToUpper();
            return GetAllTrangloDepartment().FirstOrDefault(x => x.DepartmentPrefix == deptPrefixValue);
        }

        public static readonly TrangloDepartment Finance = new TrangloDepartment(1, "Finance", "FIN");
        public static readonly TrangloDepartment SalesOperation = new TrangloDepartment(2, "Sales Operation", "SO");
        public static readonly TrangloDepartment RevenueAssurance = new TrangloDepartment(3, "Revenue Assurance", "RA");
        public static readonly TrangloDepartment Treasury = new TrangloDepartment(4, "Treasury", "TRE");
        public static readonly TrangloDepartment Product = new TrangloDepartment(5, "Product", "PROD");
        public static readonly TrangloDepartment Compliance = new TrangloDepartment(6, "Compliance", "COMP");
        public static readonly TrangloDepartment Technology = new TrangloDepartment(7, "Technology", "TECH");
        public static readonly TrangloDepartment CustomerSupport = new TrangloDepartment(8, "Customer Support", "CUST");
        public static readonly TrangloDepartment Management = new TrangloDepartment(9, "Management", "M");
        public static readonly TrangloDepartment BusinessDevelopment = new TrangloDepartment(10, "Business Development", "BD");
        public static readonly TrangloDepartment DigitalMarketing = new TrangloDepartment(11, "Digital Marketing", "DIGM");
        public static readonly TrangloDepartment ExecutiveSecretary = new TrangloDepartment(12, "Executive Secretary", "SEC");
        public static readonly TrangloDepartment HR = new TrangloDepartment(13, "HR", "HR");
        public static readonly TrangloDepartment Infrastructure = new TrangloDepartment(14, "Infrastructure", "INFR");
        public static readonly TrangloDepartment Legal = new TrangloDepartment(15, "Legal", "LEG");
        public static readonly TrangloDepartment ProcessExcellence = new TrangloDepartment(16, "Process Excellence", "PE");
        public static readonly TrangloDepartment InternalAudit = new TrangloDepartment(17, "Internal Audit", "INTA");


    }
}
