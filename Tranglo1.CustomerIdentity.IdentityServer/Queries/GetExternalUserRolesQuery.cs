using AutoMapper;
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
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.PartnerManageExternalRole, UACAction.View)]
    [Permission(Permission.ManageExternalRoles.Action_View_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { })]
    internal class GetExternalUserRolesQuery : BaseQuery<PagedResult<ExternalUserRoleListOutputDTO>>
    {
        public ExternalUserRoleListInputDTO DTO { get; set; }
        public PagingOptions PagingOptions = new PagingOptions();

        public override Task<string> GetAuditLogAsync(PagedResult<ExternalUserRoleListOutputDTO> result)
        {
            //if (result.IsSuccess)
            //{
            //    string _description = $"Get External User Role List";
            //    return Task.FromResult(_description);
            //}

            //return Task.FromResult<string>(null);

            string _description = $"Searched external roles";
            return Task.FromResult(_description);
        }
    }

    internal class GetExternalUserRolesQueryHandler : IRequestHandler<GetExternalUserRolesQuery, PagedResult<ExternalUserRoleListOutputDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public GetExternalUserRolesQueryHandler(IMapper mapper, IConfiguration config)
        {
            _mapper = mapper;
            _config = config;
        }
        private class GetExternalUserRolesView
        {
            public int Id { get; set; }
            public string RoleCode { get; set; }
            public string RoleName { get; set; }
            public int StatusCode { get; set; }
            public string Status { get; set; }
            public long SolutionCode { get; set; }
            public string SolutionDescription { get; set; }
            public int PortalCode { get; set; }
        }

        public async Task<PagedResult<ExternalUserRoleListOutputDTO>> Handle(GetExternalUserRolesQuery request, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();

            //var DTO = request.DTO;

            //PagedResult<ExternalUserRoleListOutputDTO> result = new PagedResult<ExternalUserRoleListOutputDTO>();

            //var outputDTO = new List<ExternalUserRoleListOutputDTO>();

            //var mockData = new ExternalUserRoleListOutputDTO
            //{
            //    ExternalUserRoleCode = 1,
            //    ExternalUserRoleName = "Master Teller",
            //    ExternalUserRoleStatusCode = 1               
            //};

            //outputDTO.Add(mockData);

            //PaginationInfoDTO paginationInfoDTO = new PaginationInfoDTO
            //{
            //    RowCount = 10,
            //    PageSize = 10,
            //    PageIndex = 1
            //};

            //result.RowCount = paginationInfoDTO.RowCount;
            //result.PageSize = paginationInfoDTO.PageSize;
            //result.CurrentPage = paginationInfoDTO.PageIndex;
            //result.Results = outputDTO;

            //return result;

            var dto = request.DTO;

            PagedResult<ExternalUserRoleListOutputDTO> result = new PagedResult<ExternalUserRoleListOutputDTO>();
            var outputDTO = new List<ExternalUserRoleListOutputDTO>();

            var _connectionString = _config.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var _sortExpression = string.IsNullOrEmpty(request.PagingOptions.SortExpression) ? "" : request.PagingOptions.SortExpression + " " + (request.PagingOptions.Direction == SortDirection.Ascending ? "" : "DESC");
                var reader = await connection.QueryMultipleAsync(
                    "GetExternalUserRoles",
                    new
                    {
                        PageIndex = request.PagingOptions.PageIndex,
                        PageSize = request.PagingOptions.PageSize,
                        RoleNameFilter = !string.IsNullOrWhiteSpace(dto.ExternalUserRoleName) ? string.Format("%{0}%", dto.ExternalUserRoleName) : null,
                        RoleStatusFilter = dto.ExternalUserRoleStatusCode ?? null,
                        SolutionCode = dto.SolutionCode ?? null,
                        sortExpression = _sortExpression
                    },
                    null, null, CommandType.StoredProcedure);

                var results = await reader.ReadAsync<GetExternalUserRolesView>();
                var resultsGroup = results.GroupBy(x => new { x.Id }).Select(y => y.First()).ToList();

                foreach (var r in resultsGroup)
                {
                    var partnerUsers = new ExternalUserRoleListOutputDTO()
                    {
                        ExternalUserRoleCode = r.Id,
                        RoleCode = r.RoleCode,
                        ExternalUserRoleName = r.RoleName,
                        ExternalUserRoleStatusCode = r.StatusCode,
                        ExternalUserRoleStatus = r.Status,
                        SolutionCode = r.SolutionCode,
                        SolutionDescription = r.SolutionDescription,
                        PortalCode = r.PortalCode
                    };

                    outputDTO.Add(partnerUsers);
                }

                IEnumerable<PaginationInfoDTO> _paginationInfoDTO = await reader.ReadAsync<PaginationInfoDTO>();

                result.RowCount = _paginationInfoDTO.First<PaginationInfoDTO>().RowCount;
                result.PageSize = _paginationInfoDTO.First<PaginationInfoDTO>().PageSize;
                result.CurrentPage = _paginationInfoDTO.First<PaginationInfoDTO>().PageIndex;
                result.Results = outputDTO;
            }

            return result;
        }
    }
}