using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.TrangloRole;
using Tranglo1.CustomerIdentity.IdentityServer.Helper.ACL;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.TrangloRole, UACAction.View)]
    internal class GetAdminScreenAccessListQuery : IRequest<Result<AdminScreenAccessOutputDTO>>
    {

        public class GetScreenAccessListQueryHandler : IRequestHandler<GetAdminScreenAccessListQuery, Result<AdminScreenAccessOutputDTO>>
        {
           
            private readonly IConfiguration _config;
            private AccessControlManager _accessControlManager;
            public GetScreenAccessListQueryHandler(IConfiguration config, AccessControlManager accessControlManager)
            {
                _config = config;
                _accessControlManager = accessControlManager;
            }
            public async Task<Result<AdminScreenAccessOutputDTO>> Handle(GetAdminScreenAccessListQuery request, CancellationToken cancellationToken)
            {
                
                AdminScreenAccessOutputDTO output = new AdminScreenAccessOutputDTO();

                ScreenAccessHelper screenAccessHelper = new ScreenAccessHelper(_config, _accessControlManager);

                output.ScreenAccessMenuList = await screenAccessHelper.GetScreenAccessList(PortalCode.Admin);


                return Result.Success<AdminScreenAccessOutputDTO>(output);
               
            }
        }
    }
}
