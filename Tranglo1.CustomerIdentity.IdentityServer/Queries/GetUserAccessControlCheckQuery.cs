using AutoMapper;
using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.Common.Cache;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    internal class GetUserAccessControlCheckQuery : BaseQuery<Result<bool>>
    {
        public List<string> RoleCodes { get; set; }
        public string PermissionCode { get; set; }
        public int[] PermissionPortalCodes { get; set; }
        public string UserSolutionClaim { get; set; }

        public override Task<string> GetAuditLogAsync(Result<bool> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Check Screen Access Control";
                return Task.FromResult(_description);
            }
            return Task.FromResult<string>(null);
        }
    }

    internal class GetUserAccessControlCheckQueryHandler : IRequestHandler<GetUserAccessControlCheckQuery, Result<bool>>
    {
        private readonly AccessControlManager _accessControlManager;
        private readonly IRedisCacheManager _redis;
        private readonly IConfiguration _configuration;

        public GetUserAccessControlCheckQueryHandler(AccessControlManager accessControlManager,
            IConfiguration configuration, IRedisCacheManager redis)
        {
            _accessControlManager = accessControlManager;
            _redis = redis;
            _configuration = configuration;
        }

        public async Task<Result<bool>> Handle(GetUserAccessControlCheckQuery request, CancellationToken cancellationToken)
        {

            if (_configuration.GetValue<bool>("DisableUAC"))
            {
                return Result.Success<bool>(true);
            }

            if(request.UserSolutionClaim == ClaimCode.Business)
            {
                if(request.PermissionPortalCodes.Contains((int)PortalCode.Business) == false)
                {
                    return Result.Success(true);
                }
            }
            else if(request.UserSolutionClaim == ClaimCode.Connect)
            {
                if (request.PermissionPortalCodes.Contains((int)PortalCode.Connect) == false)
                {
                    return Result.Success(true);
                }
            }
            else if(String.IsNullOrWhiteSpace(request.UserSolutionClaim))
            {
                if (request.PermissionPortalCodes.Contains((int)PortalCode.Admin) == false)
                {
                    return Result.Success(true);
                }
            }

            foreach ( var roleCode in request.RoleCodes)
            {
                Dictionary<string, string> claims = new Dictionary<string, string>();
                claims.Add("role", roleCode);

                var cahceKey = $"PermissionAssignments_{roleCode}";

                var permissionList = await _redis.GetAndSetJsonAsync<List<PermissionAssignment>>(cahceKey, (async () =>
                {
                    return await _accessControlManager.GetPermissionAssignmentsByClaims(claims);
                }));

                if (permissionList != null && permissionList.Any(o => o.PermissionInfoCode == request.PermissionCode))
                {
                    return Result.Success<bool>(true);
                }

            }
            return Result.Success<bool>(false);
        }
    }
}

