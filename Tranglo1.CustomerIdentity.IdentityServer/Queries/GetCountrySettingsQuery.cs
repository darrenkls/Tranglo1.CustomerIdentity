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
using Tranglo1.CustomerIdentity.IdentityServer.DTO.SignUpCode;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.KYCSanctionCountryManagement, UACAction.View)]
    [Permission(Permission.KYCAdministrationSanctionCountryManagement.Action_View_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] {})]
    internal class GetCountrySettingsQuery : BaseQuery<PagedResult<GetCountrySettingOutputDTO>>
    {
        public GetCountrySettingInputDTO InputDTO;
        public override Task<string> GetAuditLogAsync(PagedResult<GetCountrySettingOutputDTO> result)
        {
            return Task.FromResult("Searched High Risk / Sanctioned Country Management");
        }

        internal class GetCountrySettingsQueryHandler : IRequestHandler<GetCountrySettingsQuery, PagedResult<GetCountrySettingOutputDTO>>
        {
            private readonly IMapper _mapper;
            private readonly IConfiguration _config;

            public GetCountrySettingsQueryHandler(IMapper mapper, IConfiguration config)
            {
                _mapper = mapper;
                _config = config;
            }

            public async Task<PagedResult<GetCountrySettingOutputDTO>> Handle(GetCountrySettingsQuery request, CancellationToken cancellationToken)
            {
                PagedResult<GetCountrySettingOutputDTO> result = new PagedResult<GetCountrySettingOutputDTO>();
                var _connectionString = _config.GetConnectionString("DefaultConnection");
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                        "GetCountrySettings",
                        new
                        {
                            PageIndex = request.InputDTO.PageIndex == 0 ? 1 : request.InputDTO.PageIndex,
                            PageSize = request.InputDTO.PageSize == 0 ? 10 : request.InputDTO.PageSize,
                            CountryISO2 = request.InputDTO.CountryISO2,
                            IsHighRisk = request.InputDTO.IsHighRisk,
                            IsSanction = request.InputDTO.IsSanction,
                            IsDisplay = request.InputDTO.IsDisplay,
                            IsRejectTransaction = request.InputDTO.IsRejectTransaction
                        },
                        null, null, CommandType.StoredProcedure);

                    result.Results = await reader.ReadAsync<GetCountrySettingOutputDTO>();
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
