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
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.TrangloRole, UACAction.View)]
    internal class GetTrangloRoleListQuery : BaseQuery<Result<PagedResult<GetTrangloRoleListOutputDTO>>>
    {
        public string CreatorRoleCode { get; set; }
        public string RoleName { get; set; }
        public int? DepartmentCode { get; set; }
        public int? RoleStatusCode { get; set; }
        public int? AuthorityLevelCode { get; set; }
        public bool? IsSuperApprover { get; set; }
        public PagingOptions PagingOptions = new PagingOptions();
        public override Task<string> GetAuditLogAsync(Result<PagedResult<GetTrangloRoleListOutputDTO>> result)
        {
            if (result.IsSuccess) 
            {
                return Task.FromResult("Searched for roles");
            }

            return base.GetAuditLogAsync(result);
        }
        public class GetAdminUsersQueryHandler : IRequestHandler<GetTrangloRoleListQuery, Result<PagedResult<GetTrangloRoleListOutputDTO>>>
        {
            private readonly IConfiguration _config;

            public GetAdminUsersQueryHandler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<PagedResult<GetTrangloRoleListOutputDTO>>> Handle(GetTrangloRoleListQuery request, CancellationToken cancellationToken)
            {
                PagedResult<GetTrangloRoleListOutputDTO> result = new PagedResult<GetTrangloRoleListOutputDTO>();
                var _connectionString = _config.GetConnectionString("DefaultConnection");

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                       "dbo.GetTrangloRoles",
                       new
                       {
                           CreatorRoleCode = request.CreatorRoleCode,
                           RoleName = request.RoleName,
                           DepartmentCode = request.DepartmentCode,
                           RoleStatusCode = request.RoleStatusCode,
                           AuthorityLevelCode = request.AuthorityLevelCode,
                           IsSuperApprover = request.IsSuperApprover,
                           PageSize = request.PagingOptions.PageSize,
                           PageIndex = request.PagingOptions.PageIndex
                       },
                       null, null, CommandType.StoredProcedure);

                    result.Results = await reader.ReadAsync<GetTrangloRoleListOutputDTO>();
                    IEnumerable<PaginationInfoDTO> _paginationInfoDTO = await reader.ReadAsync<PaginationInfoDTO>();
                    result.RowCount = _paginationInfoDTO.First<PaginationInfoDTO>().RowCount;
                    result.PageSize = _paginationInfoDTO.First<PaginationInfoDTO>().PageSize;
                    result.CurrentPage = _paginationInfoDTO.First<PaginationInfoDTO>().PageIndex;

                }
                return Result.Success<PagedResult<GetTrangloRoleListOutputDTO>>(result);
            }
        }
    }
}
