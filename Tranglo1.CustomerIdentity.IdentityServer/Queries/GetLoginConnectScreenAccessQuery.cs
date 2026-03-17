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
using System.Security.Claims;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using Tranglo1.UserAccessControl;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Microsoft.Extensions.Caching.Memory;
using Tranglo1.Common.Cache;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
  
    internal class GetLoginConnectScreenAccessQuery : BaseQuery<Result<IEnumerable<UACLoginSubMenuActionOutputDTO>>>
    {
        public string RoleCode { get; set; }
        public long PartnerCode { get; set; }
        public int SolutionCode { get; set; }
        public override Task<string> GetAuditLogAsync(Result<IEnumerable<UACLoginSubMenuActionOutputDTO>> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Get Login Screen Access List based on role";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);

        }
    }

    internal class GetLoginConnectScreenAccessQueryHandler : IRequestHandler<GetLoginConnectScreenAccessQuery, Result<IEnumerable<UACLoginSubMenuActionOutputDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly AccessControlManager _accessControlManager;
        private readonly IIdentityContext _identityContext;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly TrangloUserManager _trangloUserManager;
        private readonly IRedisCacheManager _redis;

        public GetLoginConnectScreenAccessQueryHandler(IMapper mapper, IConfiguration config, AccessControlManager accessControlManager, 
            IPartnerRepository partnerRepository, IIdentityContext identityContext, IApplicationUserRepository applicationUserRepository, 
            TrangloUserManager trangloUserManager, IRedisCacheManager redis)
        {
            _mapper = mapper;
            _config = config;
            _accessControlManager = accessControlManager;
            _partnerRepository = partnerRepository;
            _applicationUserRepository = applicationUserRepository;
            _identityContext = identityContext;
            _trangloUserManager = trangloUserManager;
            _redis = redis ;
        }

        public async Task<Result<IEnumerable<UACLoginSubMenuActionOutputDTO>>> Handle(GetLoginConnectScreenAccessQuery request, CancellationToken cancellationToken)
        {
            var _connectionString = _config.GetConnectionString("UACConnection");

            IEnumerable<UACLoginSubMenuActionOutputDTO> screenAccessOutputDTOs;
            IEnumerable<UACLoginSubMenuPermissionOutputDTO> actionOutputDTOs;

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("role", request.RoleCode);
            var claims = _accessControlManager.GetClaimListing(keyValuePairs);

            var cacheKey = $"LoginAccess_{request.RoleCode}";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                actionOutputDTOs = await _redis.GetAndSetJsonAsync<IEnumerable<UACLoginSubMenuPermissionOutputDTO>>(cacheKey, (async () =>
                {
                    await connection.OpenAsync();

                    var reader = await connection.QueryMultipleAsync(
                        "GetLoginScreenAccess",
                        new
                        {
                            Claims = claims,
                            PortalCode = request.SolutionCode
                        },
                        null, null, CommandType.StoredProcedure);

                    // read as IEnumerable<dynamic>
                    actionOutputDTOs = await reader.ReadAsync<UACLoginSubMenuPermissionOutputDTO>();
                    return actionOutputDTOs;
                }));
                

            }
            var partnerCode = await _partnerRepository.GetPartnerRegistrationByCodeAsync(request.PartnerCode);
            var subscriptions = await _partnerRepository.GetSubscriptionsByPartnerCodeAsync(request.PartnerCode);

            var userId = _identityContext.CurrentUser.GetUserId().Value;
            var loginId = await _applicationUserRepository.GetTrangloUserByUserId(userId);
            ApplicationUser applicationUser = await _trangloUserManager.FindByIdAsync(loginId.LoginId);

            screenAccessOutputDTOs = actionOutputDTOs.GroupBy(k => k.SubMenuCode).Select(o =>
            new UACLoginSubMenuActionOutputDTO()
            {
                SubMenuCode = o.Key,
                UACActions = o.ToList()
            });

            var newList = screenAccessOutputDTOs.ToList();
            if (applicationUser is CustomerUser) { 
            
                if (subscriptions.All(x => x.PartnerType is null))
                {
                    var removeItem = screenAccessOutputDTOs.ToList();
                    var apiSettings = removeItem.Where(x => x.SubMenuCode.Contains("APISettings")).FirstOrDefault();

                    newList.Remove(apiSettings);
                }
            }

            return Result.Success<IEnumerable<UACLoginSubMenuActionOutputDTO>>(newList);

        }
    }
}

