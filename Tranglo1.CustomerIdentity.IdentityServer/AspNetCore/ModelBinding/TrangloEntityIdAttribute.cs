using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.AspNetCore.ModelBinding;

namespace Microsoft.AspNetCore.Mvc
{
    public class TrangloEntityIdAttribute : ModelBinderAttribute
    {
        public TrangloEntityIdAttribute()
        {
            base.BinderType = typeof(TrangloEntityModelBinder);
        }

        public override BindingSource BindingSource { get => null; protected set => base.BindingSource = value; }
    }
}
