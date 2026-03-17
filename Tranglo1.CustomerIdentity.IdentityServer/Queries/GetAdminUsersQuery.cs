using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.KYCAdminManagement.AdminManagement;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;
using static Tranglo1.CustomerIdentity.IdentityServer.Queries.TrangloUserGroupingOutputDTO.TrangloUserEntitiesGroupingOutputDTO;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{

    //[Permission(PermissionGroupCode.TrangloUser, UACAction.View)]
    [Permission(Permission.ManageAdminUser.Action_View_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] {  })]
    internal class GetAdminUsersQuery : BaseQuery<Result<PagedResult<TrangloUserGroupingOutputDTO>>>
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public int? Department { get; set; }
        public int? AccountStatus { get; set; }
        public string Entity { get; set; }
        public PagingOptions PagingOptions = new PagingOptions();
        public override Task<string> GetAuditLogAsync(Result<PagedResult<TrangloUserGroupingOutputDTO>> result)
        {
            if (result.IsSuccess) 
            {
                return Task.FromResult("Searched User Accounts");
            }

            return base.GetAuditLogAsync(result);
        }


        public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, Result<PagedResult<TrangloUserGroupingOutputDTO>>>
        { 
        private class GetAdminUsersView
        {
                public string FullName { get; set; }

                public string Email { get; set; }
                public string LoginId { get; set; }
                public string AccountStatus { get; set; }
                public string Timezone { get; set; }
                public string TrangloRole { get; set; }
                public string TrangloDepartment { get; set; }
                public string TrangloEntity { get; set; }
                public string TrangloEntityId { get; set; }
                public string BlockStatus { get; set; }
            }
        private readonly IConfiguration _config;

        public GetAdminUsersQueryHandler(IConfiguration config)
        {
            _config = config;
        }
            public async Task<Result<PagedResult<TrangloUserGroupingOutputDTO>>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
            {
                PagedResult<TrangloUserGroupingOutputDTO> result = new PagedResult<TrangloUserGroupingOutputDTO>();
                var outputDTO = new List<TrangloUserGroupingOutputDTO>();
                var _connectionString = _config.GetConnectionString("DefaultConnection");

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                       "dbo.GetAdminUsers",
                       new
                       {
                           NameFilter = request.Name,
                           RoleFilter = request.Role,
                           EntityFilter = request.Entity,
                           DepartmentFilter = request.Department,
                           AccountStatusIdFilter = request.AccountStatus,
                           PageSize = request.PagingOptions.PageSize,
                           PageIndex = request.PagingOptions.PageIndex
                       },
                       null, null, CommandType.StoredProcedure);


                   var results = await reader.ReadAsync<GetAdminUsersView>();

                    //per user
                    var userResult = results.Select(i => new { i.LoginId, i.AccountStatus,i.Timezone,i.Email,i.FullName,i.TrangloEntity}).ToList();
                    var userResultFirst = results.GroupBy(x => new { x.LoginId }).Select(x=>x.First()).ToList();

                    foreach (var u in userResultFirst)
                    {

                        var trangloUsers = new TrangloUserGroupingOutputDTO()
                        {
                            AccountStatus = u.AccountStatus,
                            Email = u.Email,
                            FullName = u.FullName,
                            LoginId = u.LoginId,
                            Timezone = u.Timezone
                        };
                        //per entity
                        var userResultGroup = results.Where(x => x.LoginId == u.LoginId).GroupBy(x => new { x.TrangloEntity }).Select(x => x.First()).ToList();
                        foreach (var r in userResultGroup) { 

                            var trangloUserEntities = new TrangloUserGroupingOutputDTO.TrangloUserEntitiesGroupingOutputDTO()
                            {
                                TrangloEntity = r.TrangloEntity,
                                TrangloEntityId = r.TrangloEntityId
                            };

                            //var userDepartmentGroups = results.GroupBy(x => new { x.LoginId, r.TrangloEntityId, r.TrangloRole }).Select(x=>x.First()).ToList();
                            var userRolesResult = results.Where(x => x.TrangloEntity == r.TrangloEntity && x.LoginId == r.LoginId);
                                                              /*.GroupBy(x => new { x.LoginId, x.TrangloEntity, x.BlockStatus, x.TrangloDepartment, x.TrangloRole })
                                                              .Select(x => new TrangloUserRoleGroupingOutputDTO
                                                              {
                                                                  TrangloDepartment = x.TrangloDepartment,
                                                                  TrangloRole = x.Key.TrangloRole,
                                                                  BlockStatus = x.Key.BlockStatus
                                                              });
                                                              */
                            foreach ( var role in userRolesResult)
                            {
                                var trangloUserRoles = new TrangloUserGroupingOutputDTO.TrangloUserEntitiesGroupingOutputDTO.TrangloUserRoleGroupingOutputDTO
                                {
                                    TrangloDepartment = role.TrangloDepartment,// r.TrangloDepartment,
                                    TrangloRole = role.TrangloRole,
                                    BlockStatus = role.BlockStatus
                                };
                                trangloUserEntities.RoleGrouping.Add(trangloUserRoles);
                            }
                            trangloUsers.Entities.Add(trangloUserEntities);
                        }
                      
                        outputDTO.Add(trangloUsers);                       
                    }

                    IEnumerable<PaginationInfoDTO> _paginationInfoDTO = await reader.ReadAsync<PaginationInfoDTO>();
                    result.RowCount = _paginationInfoDTO.First<PaginationInfoDTO>().RowCount;
                    result.PageSize = _paginationInfoDTO.First<PaginationInfoDTO>().PageSize;
                    result.CurrentPage = _paginationInfoDTO.First<PaginationInfoDTO>().PageIndex;
                    result.Results = outputDTO;
                }
                return Result.Success<PagedResult<TrangloUserGroupingOutputDTO>>(result);
            }
        }
    }
}
