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
    internal class GetAllExternalUserRoleQuery : BaseQuery<Result<IEnumerable<ExternalRoleListOutputDTO>>>
    {
        public override Task<string> GetAuditLogAsync(Result<IEnumerable<ExternalRoleListOutputDTO>> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Get All External User Role";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
    }

    internal class GetAllExternalUserRoleQueryHandler : IRequestHandler<GetAllExternalUserRoleQuery, Result<IEnumerable<ExternalRoleListOutputDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private AccessControlManager _accessControlManager;
        private readonly IExternalUserRoleRepository _externalUserRoleRepository;

        public GetAllExternalUserRoleQueryHandler(IMapper mapper, IConfiguration config, AccessControlManager accessControlManager, IExternalUserRoleRepository externalUserRoleRepository)
        {
            _mapper = mapper;
            _config = config;
            _accessControlManager = accessControlManager;
            _externalUserRoleRepository = externalUserRoleRepository;
        }

        public async Task<Result<IEnumerable<ExternalRoleListOutputDTO>>> Handle(GetAllExternalUserRoleQuery request, CancellationToken cancellationToken)
        {
            var result = _externalUserRoleRepository.GetAllExternalUserRoles();

            var outputDTO = _mapper.Map<IEnumerable<ExternalRoleListOutputDTO>>(result);

            return Result.Success<IEnumerable<ExternalRoleListOutputDTO>>(outputDTO);
        }
    }
}
