using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class TrangloEntity : Enumeration
    {
        public string TrangloEntityCode { get; set; }
        public TrangloEntity() 
        {

        }
        public TrangloEntity(int id, string name, string code) : base(id,name)            
        {
            
            this.TrangloEntityCode = code;
      
        }

        public static IEnumerable<TrangloEntity> GetTrangloEntities()
        {
            return _CachedEnumerations.GetOrAdd(typeof(TrangloEntity), t =>
            {
                var fields = typeof(TrangloEntity).GetFields(BindingFlags.Public |
                                                             BindingFlags.Static |
                                                             BindingFlags.DeclaredOnly);

                return fields
                    .Where(f => f.DeclaringType == t)
                    .Select(f => f.GetValue(null))
                    .Cast<TrangloEntity>()
                    .OrderBy(e => e.Name)
                    .ToArray();

            }).Cast<TrangloEntity>();
        }

        public static TrangloEntity GetByEntityByTrangloId(string trangloCode)
        {
            var trangloCodeValue = trangloCode?.ToUpper();
            return GetTrangloEntities().FirstOrDefault(x => x.TrangloEntityCode == trangloCodeValue);
        }

        public static readonly TrangloEntity TSB = new TrangloEntity(1, "Tranglo Sdn Bhd", "TSB");
        public static readonly TrangloEntity TPL = new TrangloEntity(2, "Tranglo Pte Ltd","TPL");
        public static readonly TrangloEntity TEL = new TrangloEntity(3, "Tranglo Europe Ltd","TEL");
        public static readonly TrangloEntity PTT = new TrangloEntity(4, "PT Tranglo Indonesia","PTT");
    }
}
