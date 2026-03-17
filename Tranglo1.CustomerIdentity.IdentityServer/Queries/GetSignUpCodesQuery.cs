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
    //[Permission(PermissionGroupCode.PartnerSignUpCode, UACAction.View)]
    [Permission(Permission.SignupCode.Action_View_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.RegisterNewPartner.Action_View_Code }
        )]

    internal class GetSignUpCodesQuery : PagingOptions, IRequest<PagedResult<SignUpCodesOutputDTO>>
    {
        public string TrangloEntity { get; set; }
    }
        internal class GetSignUpCodesQueryHandler : IRequestHandler<GetSignUpCodesQuery, PagedResult<SignUpCodesOutputDTO>>
        {
            private readonly IMapper _mapper;
            private readonly IConfiguration _config;
        
            public GetSignUpCodesQueryHandler(IMapper mapper, IConfiguration config)
            {
                _mapper = mapper;
                _config = config;
            }

             public async Task<PagedResult<SignUpCodesOutputDTO>>Handle(GetSignUpCodesQuery request, CancellationToken cancellationToken)
            {
                PagedResult<SignUpCodesOutputDTO> result = new PagedResult<SignUpCodesOutputDTO>();
                var _connectionString = _config.GetConnectionString("DefaultConnection");
                var _sortExpression = string.IsNullOrEmpty(request.SortExpression) ? "" : request.SortExpression + " " + (request.Direction == SortDirection.Ascending ? "" : "DESC");

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();   
                    var reader = await connection.QueryMultipleAsync(
                        "GetSignUpCode",
                        new
                        {
                            PageIndex = request.PageIndex,
                            PageSize = request.PageSize,
                            sortExpression = _sortExpression,
                            TrangloEntity = request.TrangloEntity
                        },
                        null, null, CommandType.StoredProcedure);

                result.Results = await reader.ReadAsync<SignUpCodesOutputDTO>();
                IEnumerable<PaginationInfoDTO> _paginationInfoDTO = await reader.ReadAsync<PaginationInfoDTO>();
                result.RowCount = _paginationInfoDTO.First<PaginationInfoDTO>().RowCount;
                result.PageSize = _paginationInfoDTO.First<PaginationInfoDTO>().PageSize;
                result.CurrentPage = _paginationInfoDTO.First<PaginationInfoDTO>().PageIndex;
            }

            return result;
        }
        }
    }
