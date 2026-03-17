using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Controllers.Version
{
    [Route("api")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        [HttpGet("version")]
		[ValidateAntiForgeryToken]
		public string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var build_version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            return build_version;
        }


        [HttpGet("product-version")]
        public string GetProductVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            string product_version = "1.0.0";

            int charLocation = version.IndexOf("-", StringComparison.Ordinal);

            if (charLocation > 0)
            {
                product_version = version.Substring(0, charLocation);
            }
            return product_version;
        }
    }
}
