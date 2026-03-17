using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Extensions;
using Tranglo1.CustomerIdentity.IdentityServer.Helper;

namespace Tranglo1.CustomerIdentity.IdentityServer.AspNetCore.Middlewares
{
    public class FileUploadSecurityMiddleware
    {
        private readonly RequestDelegate _next;

        public FileUploadSecurityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method != "POST"
                && context.Request.Method != "PUT"
                && context.Request.Method != "PATCH")
            {
                await _next(context);

                return;
            }

            if (context.Request.ContentType == null
                || !context.Request.ContentType.Contains("multipart/form-data"))
            {
                await _next(context);

                return;
            }

            var form = await context.Request.ReadFormAsync();
            var files = form.Files;

            if (files == null || files.Count == 0)
            {
                await _next(context);

                return;
            }

            foreach (var file in files)
            {
                Result<bool> fileHasAcceptedExtensionResult = FileHelper.ValidateFileHasAcceptedExtension(file);
                if (fileHasAcceptedExtensionResult.IsFailure)
                {
                    await WriteValidationProblemAsync(context, detail: fileHasAcceptedExtensionResult.Error);
                    return;
                }

                Result<bool> fileHasSingleExtensionResult = FileHelper.ValidateFileHasSingleExtension(file);
                if (fileHasSingleExtensionResult.IsFailure)
                {
                    await WriteValidationProblemAsync(context, detail: fileHasSingleExtensionResult.Error);

                    return;
                }

                Result<bool> fileHasTrueMimeResult = FileHelper.ValidateFileHasTrueExtension(file);
                if (fileHasTrueMimeResult.IsFailure)
                {
                    await WriteValidationProblemAsync(context, detail: fileHasTrueMimeResult.Error);

                    return;
                }
            }

            await _next(context);
        }

        private static async Task WriteValidationProblemAsync(HttpContext context, int? statusCode = null, string title = null, string detail = null)
        {
            var factory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();

            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Error", detail);

            var problem = factory.CreateValidationProblemDetails(
                context,
                modelState,
                statusCode: statusCode,
                title: title,
                detail: detail
            );

            context.Response.StatusCode = problem.Status.GetValueOrDefault();
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
