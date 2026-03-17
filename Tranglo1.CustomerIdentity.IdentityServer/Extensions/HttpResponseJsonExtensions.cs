using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Extensions
{
    public static class HttpResponseJsonExtensions
    {
        /// <summary>
        /// Return response with JSON content type.
        /// <br /><br />
        /// <em>
        /// TO-DO: Migrate to <a href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpresponsejsonextensions.writeasjsonasync?view=aspnetcore-5.0#microsoft-aspnetcore-http-httpresponsejsonextensions-writeasjsonasync(microsoft-aspnetcore-http-httpresponse-system-object-system-type-system-threading-cancellationtoken)">
        ///     <c>HttpResponseJsonExtensions.WriteAsJsonAsync</c>
        /// </a> method after upgrade to .NET 5.0 and above.
        /// </em>
        /// </summary>
        /// <param name="response"></param>
        /// <param name="o"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task WriteAsJsonAsync(this HttpResponse response, object o, string contentType = null)
        {
            var json = JsonSerializer.Serialize(o);

            response.ContentType = contentType ?? response.ContentType ?? "application/json";
            await response.WriteAsync(json);
            await response.Body.FlushAsync();
        }
    }
}
