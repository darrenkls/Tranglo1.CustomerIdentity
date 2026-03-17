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
    public class GetAdminPermissionInfoQuery : IRequest<IEnumerable<AdminPermissionInfoOutputDTO>>
    {
        public class GetAdminPermissionInfoQueryHandler : IRequestHandler<GetAdminPermissionInfoQuery, IEnumerable<AdminPermissionInfoOutputDTO>>
        {
            private readonly IConfiguration _config;
            public GetAdminPermissionInfoQueryHandler(IConfiguration config)
            {
                _config = config;
            }

            public async Task<IEnumerable<AdminPermissionInfoOutputDTO>> Handle(GetAdminPermissionInfoQuery request, CancellationToken cancellationToken)
            {
                var _connectionString = _config.GetConnectionString("DefaultConnection");
                var listDTO = new List<AdminPermissionInfoOutputDTO>();
                var dto = new AdminPermissionInfoOutputDTO();
                var dto2 = new AdminPermissionInfoOutputDTO();
                var dto3 = new AdminPermissionInfoOutputDTO();
                var dto4 = new AdminPermissionInfoOutputDTO();
                var dto5 = new AdminPermissionInfoOutputDTO();
                var dto6 = new AdminPermissionInfoOutputDTO();
                var dto7 = new AdminPermissionInfoOutputDTO();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                        "GetPermissionInfoAdmin",
                        new
                        {

                        },
                        null, null, CommandType.StoredProcedure);
                    var Compliance = await reader.ReadAsync<GetAdminPermissionInfoOutputDTO>();
                    var Kyc = await reader.ReadAsync<GetAdminPermissionInfoOutputDTO>();
                    var Partner = await reader.ReadAsync<GetAdminPermissionInfoOutputDTO>();
                    var ApiSetting = await reader.ReadAsync<GetAdminPermissionInfoOutputDTO>();
                    var Business = await reader.ReadAsync<GetAdminPermissionInfoOutputDTO>();
                    var Administration = await reader.ReadAsync<GetAdminPermissionInfoOutputDTO>();
                    var TrangloUser = await reader.ReadAsync<GetAdminPermissionInfoOutputDTO>();

                    var menu1 = Compliance.Select(x => x.Menu).FirstOrDefault();
                    dto.MainMenu = menu1;
                    foreach (var kyc in Compliance)
                    {

                        var permissionAction = new PermissionInfoAction();
                        permissionAction.Edit = kyc.IsEdit;
                        permissionAction.View = kyc.IsView;
                        permissionAction.Create = kyc.IsCreate;
                        permissionAction.Approve = kyc.IsApprove;
                        permissionAction.PermissionGroup = kyc.PermissionGroup;
                        dto.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto);
                    var menu2 = Kyc.Select(x => x.Menu).FirstOrDefault();
                    dto2.MainMenu = menu2;
                    foreach (var kyc in Kyc)
                    {

                        var permissionAction = new PermissionInfoAction();
                        permissionAction.Edit = kyc.IsEdit;
                        permissionAction.View = kyc.IsView;
                        permissionAction.Create = kyc.IsCreate;
                        permissionAction.Approve = kyc.IsApprove;
                        permissionAction.PermissionGroup = kyc.PermissionGroup;
                        dto2.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto2);

                    var menu3 = Partner.Select(x => x.Menu).FirstOrDefault();
                    dto3.MainMenu = menu3;
                    foreach (var partner in Partner)
                    {

                        var permissionAction = new PermissionInfoAction();
                        permissionAction.Edit = partner.IsEdit;
                        permissionAction.View = partner.IsView;
                        permissionAction.Create = partner.IsCreate;
                        permissionAction.Approve = partner.IsApprove;
                        permissionAction.PermissionGroup = partner.PermissionGroup;
                        dto3.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto3);

                    var menu4 = ApiSetting.Select(x => x.Menu).FirstOrDefault();
                    dto4.MainMenu = menu4;
                    foreach (var apiSetting in ApiSetting)
                    {

                        var permissionAction = new PermissionInfoAction();
                        permissionAction.Edit = apiSetting.IsEdit;
                        permissionAction.View = apiSetting.IsView;
                        permissionAction.Create = apiSetting.IsCreate;
                        permissionAction.Approve = apiSetting.IsApprove;
                        permissionAction.PermissionGroup = apiSetting.PermissionGroup;
                        dto4.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto4);

                    var menu5 = Business.Select(x => x.Menu).FirstOrDefault();
                    dto5.MainMenu = menu5;
                    foreach (var businessPricing in Business)
                    {

                        var permissionAction = new PermissionInfoAction();
                        permissionAction.Edit = businessPricing.IsEdit;
                        permissionAction.View = businessPricing.IsView;
                        permissionAction.Create = businessPricing.IsCreate;
                        permissionAction.Approve = businessPricing.IsApprove;
                        permissionAction.PermissionGroup = businessPricing.PermissionGroup;
                        dto5.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto5);

                    var menu6 = Administration.Select(x => x.Menu).FirstOrDefault();
                    dto6.MainMenu = menu6;
                    foreach (var admin in Administration)
                    {

                        var permissionAction = new PermissionInfoAction();
                        permissionAction.Edit = admin.IsEdit;
                        permissionAction.View = admin.IsView;
                        permissionAction.Create = admin.IsCreate;
                        permissionAction.Approve = admin.IsApprove;
                        permissionAction.PermissionGroup = admin.PermissionGroup;
                        dto6.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto6);

                    var menu7 = TrangloUser.Select(x => x.Menu).FirstOrDefault();
                    dto7.MainMenu = menu7;
                    foreach (var user in TrangloUser)
                    {

                        var permissionAction = new PermissionInfoAction();
                        permissionAction.Edit = user.IsEdit;
                        permissionAction.View = user.IsView;
                        permissionAction.Create = user.IsCreate;
                        permissionAction.Approve = user.IsApprove;
                        permissionAction.PermissionGroup = user.PermissionGroup;
                        dto7.PermissionInfoActions.Add(permissionAction);
                    }
                    listDTO.Add(dto7);
                    return listDTO;
                }
            }
        }
    }
}
