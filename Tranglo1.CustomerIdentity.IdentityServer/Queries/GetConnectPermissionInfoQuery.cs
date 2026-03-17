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
using Tranglo1.CustomerIdentity.IdentityServer.DTO.TrangloRole;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetConnectPermissionInfoQuery : IRequest<IEnumerable<ConnectPermissionInfoOutputDTO>>
    {
        public long SolutionCode { get; set; }

        public class GetConnectPermissionInfoQueryHandler : IRequestHandler<GetConnectPermissionInfoQuery, IEnumerable<ConnectPermissionInfoOutputDTO>>
        {
            private readonly IConfiguration _config;
        
            public GetConnectPermissionInfoQueryHandler(IConfiguration config)
            {
                _config = config;
            }

            public async Task<IEnumerable<ConnectPermissionInfoOutputDTO>> Handle(GetConnectPermissionInfoQuery request, CancellationToken cancellationToken)
            {
                var portalCode = 0;

                if(request.SolutionCode == 2)
                {
                    portalCode = 3; // Description : Tranglo Business
                }
                if(request.SolutionCode == 1) 
                {
                    portalCode = 1; // Description : Tranglo Connect
                }

                var _connectionString = _config.GetConnectionString("DefaultConnection");
                var listDTO = new List<ConnectPermissionInfoOutputDTO>();
                var dto = new ConnectPermissionInfoOutputDTO();
                var dto2 = new ConnectPermissionInfoOutputDTO();
                var dto3 = new ConnectPermissionInfoOutputDTO();
                var dto4 = new ConnectPermissionInfoOutputDTO();
                var dto5 = new ConnectPermissionInfoOutputDTO();
                var dto6 = new ConnectPermissionInfoOutputDTO();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                        "GetPermissionInfoConnect",
                        new
                        {
                            PortalCode = portalCode
                        },
                        null, null, CommandType.StoredProcedure);
                    var Team = await reader.ReadAsync<GetConnectPermissionInfoOutputDTO>();
                    var Home = await reader.ReadAsync<GetConnectPermissionInfoOutputDTO>();
                    var ApiSettings = await reader.ReadAsync<GetConnectPermissionInfoOutputDTO>();
                    var Business = await reader.ReadAsync<GetConnectPermissionInfoOutputDTO>();
                    var Agreement = await reader.ReadAsync<GetConnectPermissionInfoOutputDTO>();
                    var Kyc = await reader.ReadAsync<GetConnectPermissionInfoOutputDTO>();

                    var menu1 = Team.Select(x => x.Menu).FirstOrDefault();
                    dto.MainMenu = menu1;
                    foreach (var team in Team)
                    {

                        var permissionAction = new PermissionInfoAction2();
                        permissionAction.Edit = team.IsEdit;
                        permissionAction.View = team.IsView;
                        permissionAction.Create = team.IsCreate;
                        permissionAction.Approve = team.IsApprove;
                        permissionAction.PermissionGroup = team.PermissionGroup;
                        dto.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto);

                    var menu2 = Home.Select(x => x.Menu).FirstOrDefault();
                    dto2.MainMenu = menu2;
                    foreach (var home in Home)
                    {

                        var permissionAction = new PermissionInfoAction2();
                        permissionAction.Edit = home.IsEdit;
                        permissionAction.View = home.IsView;
                        permissionAction.Create = home.IsCreate;
                        permissionAction.Approve = home.IsApprove;
                        permissionAction.PermissionGroup = home.PermissionGroup;
                        dto2.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto2);

                    var menu3 = ApiSettings.Select(x => x.Menu).FirstOrDefault();
                    dto3.MainMenu = menu3;
                    foreach (var api in ApiSettings)
                    {

                        var permissionAction = new PermissionInfoAction2();
                        permissionAction.Edit = api.IsEdit;
                        permissionAction.View = api.IsView;
                        permissionAction.Create = api.IsCreate;
                        permissionAction.Approve = api.IsApprove;
                        permissionAction.PermissionGroup = api.PermissionGroup;
                        dto3.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto3);

                    var menu4 = Business.Select(x => x.Menu).FirstOrDefault();
                    dto4.MainMenu = menu4;
                    foreach (var businessPricing in Business)
                    {

                        var permissionAction = new PermissionInfoAction2();
                        permissionAction.Edit = businessPricing.IsEdit;
                        permissionAction.View = businessPricing.IsView;
                        permissionAction.Create = businessPricing.IsCreate;
                        permissionAction.Approve = businessPricing.IsApprove;
                        permissionAction.PermissionGroup = businessPricing.PermissionGroup;
                        dto4.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto4);

                    var menu5 = Agreement.Select(x => x.Menu).FirstOrDefault();
                    dto5.MainMenu = menu5;
                    foreach (var agreement in Agreement)
                    {

                        var permissionAction = new PermissionInfoAction2();
                        permissionAction.Edit = agreement.IsEdit;
                        permissionAction.View = agreement.IsView;
                        permissionAction.Create = agreement.IsCreate;
                        permissionAction.Approve = agreement.IsApprove;
                        permissionAction.PermissionGroup = agreement.PermissionGroup;
                        dto5.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto5);

                    var menu6 = Kyc.Select(x => x.Menu).FirstOrDefault();
                    dto6.MainMenu = menu6;
                    foreach (var kyc in Kyc)
                    {

                        var permissionAction = new PermissionInfoAction2();
                        permissionAction.Edit = kyc.IsEdit;
                        permissionAction.View = kyc.IsView;
                        permissionAction.Create = kyc.IsCreate;
                        permissionAction.Approve = kyc.IsApprove;
                        permissionAction.PermissionGroup = kyc.PermissionGroup;
                        dto6.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto6);
                    return listDTO;
                }
            }
        }
    }
}
