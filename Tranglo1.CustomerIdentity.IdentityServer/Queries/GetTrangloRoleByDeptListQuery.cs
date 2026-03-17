using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;
namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetTrangloRoleByDeptListQuery : IRequest<Result<IEnumerable<GetTrangloRoleByDeptListOutputDTO>>>
    {
        public int DepartmentId { get; set; }
        public class GetTrangloRoleListQueryHandler : IRequestHandler<GetTrangloRoleByDeptListQuery, Result<IEnumerable<GetTrangloRoleByDeptListOutputDTO>>>
        {
            
            private readonly ApplicationUserDbContext _context;
            private readonly ITrangloRoleRepository _repository;
            private readonly IMapper _mapper;
            public GetTrangloRoleListQueryHandler(ApplicationUserDbContext context, IMapper mapper, ITrangloRoleRepository repository)
            {
                _context = context;
                _mapper = mapper;
                _repository = repository;
            }
            public async Task<Result<IEnumerable<GetTrangloRoleByDeptListOutputDTO>>> Handle(GetTrangloRoleByDeptListQuery query, CancellationToken cancellationToken)
            {
                var roleDetails = await _repository.GetRolesInDepartment(query.DepartmentId);

                var result = _mapper.Map<IEnumerable<GetTrangloRoleByDeptListOutputDTO>>(roleDetails);

                return Result.Success<IEnumerable<GetTrangloRoleByDeptListOutputDTO>>(result);
            }
        }
    }
}
