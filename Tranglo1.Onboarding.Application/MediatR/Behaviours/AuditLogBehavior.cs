using CorrelationId;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.CustomerIdentity.IdentityServer.Security;
using Tranglo1.CustomerIdentity.IdentityServer.Services;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using Tranglo1.UserAccessControl;

namespace MediatR
{
    class AuditLogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public AuditLogBehavior(IAuditLogManager auditLogManager,
            IHttpContextAccessor httpContextAccessor,
            IIdentityContext identityContext,
            ICorrelationContextAccessor correlationContextAccessor)
        {
            AuditLogManager = auditLogManager;
            HttpContextAccessor = httpContextAccessor;
            IdentityContext = identityContext;
            CorrelationContextAccessor = correlationContextAccessor;

        }

        /// <summary>
		/// Suggest to use <see cref="IAuditLogService"/> instead of <see cref="IAuditLogManager"/> to centralized the audit log mechanism
		/// </summary>
        public IAuditLogManager AuditLogManager { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public IIdentityContext IdentityContext { get; }
        public ICorrelationContextAccessor CorrelationContextAccessor { get; }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();

            if (!RequestContext.IsUacRequest)
            {
                var command = request as BaseRequest<TResponse>;
                if (command != null)
                {
                    var _AuditLog = await command.GetAuditLogAsync(response);
                    if (string.IsNullOrEmpty(_AuditLog) == false)
                    {
                        var _ClientIp = HttpContextAccessor.HttpContext?.Connection?.RemoteIpAddress;
                        var _CurrentUser = IdentityContext.CurrentUser;

                        var _UserId = _CurrentUser.GetSubjectId();

                        var permissionAttribute = PermissionAttributeRetrieval.GetPermissionAttribute<TRequest>();

                        AuditLog auditLog = new AuditLog()
                        {
                            ClientAddress = _ClientIp,
                            ActionDescription = _AuditLog,
                            EventDate = DateTime.UtcNow,
                            ModuleName = permissionAttribute?.Code,
                            Username = _UserId.HasValue ? _UserId.Value : "System",
                            UserType = _CurrentUser.GetUserType(),
                            CorrelationId = CorrelationContextAccessor.CorrelationContext.CorrelationId,
                        };

                        await AuditLogManager.LogAsync(auditLog, cancellationToken);
                    }
                }
            }

            return response;
        }
    }
}
