using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
	public class PartnerCodeAttribute : ModelBinderAttribute
	{
		public PartnerCodeAttribute()
		{
			base.BinderType = typeof(PartnerCodeModelBinder);
		}

        public override BindingSource BindingSource { get => null; protected set => base.BindingSource = value; }
    }
}
