using System.Collections.Generic;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;

namespace Tranglo1.CustomerIdentity.Infrastructure.Services
{
    public class StaffEntityQueryService : IStaffEntityQueryService
    {
        private readonly IApplicationUserRepository applicationUserRepository;

        public StaffEntityQueryService(IApplicationUserRepository applicationUserRepository)
        {
            this.applicationUserRepository = applicationUserRepository;
        }

        public async Task<List<TrangloStaffEntityAssignment>> GetTrangloStaffEntityAssignmentById(string loginId)
        {
            return await applicationUserRepository.GetTrangloStaffEntityAssignmentById(loginId);
        }
    }
}
