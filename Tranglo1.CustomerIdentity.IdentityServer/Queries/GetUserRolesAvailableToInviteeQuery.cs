using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    ////[Permission("APIGetUserRolesAvailableToInviteeQuery", "Get User Role List", "UserRolesAvailableToInviteeListing")]
    public class GetUserRolesAvailableToInviteeQuery : IRequest<Result<IReadOnlyList<UserRolesOutputDTO>>>
    {
        public class GetUserRolesAvailableToInviteeQueryHandler : IRequestHandler<GetUserRolesAvailableToInviteeQuery, Result<IReadOnlyList<UserRolesOutputDTO>>>
        {
            private readonly ApplicationUserDbContext _context;
            private readonly IMapper _mapper;

            public GetUserRolesAvailableToInviteeQueryHandler(ApplicationUserDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<IReadOnlyList<UserRolesOutputDTO>>> Handle(GetUserRolesAvailableToInviteeQuery query, CancellationToken cancellationToken)
            {                
                var roleList = await _context.UserRoles.ProjectTo<UserRolesOutputDTO>(_mapper.ConfigurationProvider)
                               .Where(c => c.UserRoleCode != UserRole.MasterTeller.Id)
                               .ToListAsync(cancellationToken);

                if (roleList != null)
                {
                    return Result.Success<IReadOnlyList<UserRolesOutputDTO>>(roleList);
                }

                return Result.Failure<IReadOnlyList<UserRolesOutputDTO>>("User role list is empty.");
            }
        }
    }
}
