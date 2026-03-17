using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.UserAccessControl;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.PartnerUser, UACAction.View)]
    public class GetTeamUserListQuery : PagingOptions, IRequest<PagedResult<TeamUserListOutputDTO>>
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Environment { get; set; }
        public int? StatusCode { get; set; }
        public string RoleCode { get; set; }
        public int BusinessProfileCode { get; set; }


        public class GetTeamUserListQueryHandler : IRequestHandler<GetTeamUserListQuery, PagedResult<TeamUserListOutputDTO>>
        {
            private readonly IConfiguration _config;
            public GetTeamUserListQueryHandler(IConfiguration config)
            {
                _config = config;
            }

            public async Task<PagedResult<TeamUserListOutputDTO>> Handle(GetTeamUserListQuery request, CancellationToken cancellationToken)
            {
                PagedResult<TeamUserListOutputDTO> result = new PagedResult<TeamUserListOutputDTO>();
                var _connectionString = _config.GetConnectionString("DefaultConnection");
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var _sortExpression = string.IsNullOrEmpty(request.SortExpression) ? "" : request.SortExpression + " " + (request.Direction == SortDirection.Ascending ? "" : "DESC");
                    var reader = await connection.QueryMultipleAsync(
                        "GetTeamUserListQuery",
                        new
                        {
                            PageIndex = request.PageIndex,
                            PageSize = request.PageSize,
                            BusinessProfileCode = request.BusinessProfileCode,
                            Name = !string.IsNullOrWhiteSpace(request.Name) ? string.Format("%{0}%", request.Name) : null,
                            AccountStatusCode = (request.StatusCode.HasValue) ? request.StatusCode.Value : 0,
                            RoleCode = !string.IsNullOrWhiteSpace(request.RoleCode) ? request.RoleCode : null,
                            UserId = request.UserId,
                            sortExpression = _sortExpression
                        },
                        null, null, CommandType.StoredProcedure);
                    result.Results = await reader.ReadAsync<TeamUserListOutputDTO>();
                    IEnumerable<PaginationInfoDTO> _paginationInfoDTO = await reader.ReadAsync<PaginationInfoDTO>();
                    result.RowCount = _paginationInfoDTO.First<PaginationInfoDTO>().RowCount;
                    result.PageSize = _paginationInfoDTO.First<PaginationInfoDTO>().PageSize;
                    result.CurrentPage = _paginationInfoDTO.First<PaginationInfoDTO>().PageIndex;
                }
                return result;
            }
        }
    }
}
