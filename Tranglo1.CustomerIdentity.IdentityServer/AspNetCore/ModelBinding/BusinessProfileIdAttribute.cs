using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
	public class BusinessProfileIdAttribute : ModelBinderAttribute
	{
		public BusinessProfileIdAttribute()
		{
			base.BinderType = typeof(BusinessProfileIdModelBinder);
		}

        public override BindingSource BindingSource { get => null; protected set => base.BindingSource = value; }
    }
}
