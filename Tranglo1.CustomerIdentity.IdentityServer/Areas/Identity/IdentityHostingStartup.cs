using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Tranglo1.CustomerIdentity.IdentityServer.Areas.Identity.IdentityHostingStartup))]
namespace Tranglo1.CustomerIdentity.IdentityServer.Areas.Identity
{
	public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}