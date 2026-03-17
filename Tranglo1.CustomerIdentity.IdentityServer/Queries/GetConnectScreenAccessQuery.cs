using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.Helper.ACL;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.PartnerManageExternalRole, UACAction.View)]
    internal class GetConnectScreenAccessQuery : BaseQuery<Result<ConnectScreenAccessOutputDTO>>
    {
        public long SolutionCode { get; set; }

        public override Task<string> GetAuditLogAsync(Result<ConnectScreenAccessOutputDTO> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Get Screen Access List";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);

        }
    }

    internal class GetConnectScreenAccessQueryHandler : IRequestHandler<GetConnectScreenAccessQuery, Result<ConnectScreenAccessOutputDTO>>
    {
        private readonly IConfiguration _config;
        public GetConnectScreenAccessQueryHandler(IConfiguration config)
        {
            _config = config;
        }

        public async Task<Result<ConnectScreenAccessOutputDTO>> Handle(GetConnectScreenAccessQuery request, CancellationToken cancellationToken)
        {
            var output = new ConnectScreenAccessOutputDTO();

            ScreenAccessHelper screenAccessHelper = new ScreenAccessHelper(_config);

            output.ScreenAccessMenuList = await screenAccessHelper.GetScreenAccessList((PortalCode)request.SolutionCode);

            return Result.Success(output);

        }
    }
}

