using Microsoft.AspNetCore.Builder;
using Tranglo1.CustomerIdentity.IdentityServer.AspNetCore.Middlewares;

namespace Tranglo1.CustomerIdentity.IdentityServer.AspNetCore.Builder
{
    public static class FileUploadSecurityMiddlewareExtensions
    {
        public static IApplicationBuilder UseFileUploadSecurity(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<FileUploadSecurityMiddleware>();

            return builder;
        }
    }
}
