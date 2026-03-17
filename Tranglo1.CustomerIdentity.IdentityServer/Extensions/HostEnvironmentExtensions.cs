using Microsoft.Extensions.Hosting;

namespace Tranglo1.CustomerIdentity.IdentityServer.Extensions
{
	public static class HostEnvironmentExtensions
	{
		public static bool IsDevelopmentEnv(this IHostEnvironment hostEnvironment)
		{
			return hostEnvironment.IsEnvironment("localDevelopment")
				|| hostEnvironment.IsEnvironment("localDevelopmentP2")
				|| hostEnvironment.IsEnvironment("developmentP2")
				|| hostEnvironment.IsEnvironment("Local");
        }

		public static bool IsQA(this IHostEnvironment hostEnvironment)
		{
			return hostEnvironment.IsEnvironment("localQA")
				|| hostEnvironment.IsEnvironment("localQAP2")
				|| hostEnvironment.IsEnvironment("localUATP2")
				|| hostEnvironment.IsEnvironment("QAP2");
		}
	}
}
