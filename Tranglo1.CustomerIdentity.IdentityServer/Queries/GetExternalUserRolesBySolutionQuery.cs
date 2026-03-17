using AutoMapper;
using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    internal class GetExternalUserRolesBySolutionQuery : BaseQuery<Result<IEnumerable<ExternalRoleListOutputDTO>>>
    {
        public long SolutionCode { get; set; }

        public override Task<string> GetAuditLogAsync(Result<IEnumerable<ExternalRoleListOutputDTO>> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Get External User Role By Solution";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
    }

    internal class GetExternalUserRolesBySolutionQueryHandler : IRequestHandler<GetExternalUserRolesBySolutionQuery, Result<IEnumerable<ExternalRoleListOutputDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IExternalUserRoleRepository _externalUserRoleRepository;

        public GetExternalUserRolesBySolutionQueryHandler(IMapper mapper, IExternalUserRoleRepository externalUserRoleRepository)
        {
            _mapper = mapper;
            _externalUserRoleRepository = externalUserRoleRepository;
        }

        public async Task<Result<IEnumerable<ExternalRoleListOutputDTO>>> Handle(GetExternalUserRolesBySolutionQuery request, CancellationToken cancellationToken)
        {
            var result = await _externalUserRoleRepository.GetAllExternalUserRolesBySolution(request.SolutionCode);
            var outputDTO = _mapper.Map<IEnumerable<ExternalRoleListOutputDTO>>(result);

            return Result.Success(outputDTO);
        }
    }
}