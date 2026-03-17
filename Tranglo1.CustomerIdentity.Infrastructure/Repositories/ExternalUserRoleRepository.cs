using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.ExternalUserRoleAggregate;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.Repositories
{
    public class ExternalUserRoleRepository : IExternalUserRoleRepository
    {
        private readonly ExternalUserRoleDbContext _dbContext;

        public ExternalUserRoleRepository(ExternalUserRoleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ExternalUserRole[] GetAllExternalUserRoles()
        {
            return this._dbContext.ExternalUserRoles.ToArray();         
        }

        public async Task<ExternalUserRole> GetExternalRoleByRoleCodeAsync(string roleCode)
        {
            return await _dbContext.ExternalUserRoles
                .Include(r => r.Solution)
                .FirstOrDefaultAsync(x => x.RoleCode == roleCode);
        }
        public async Task<ExternalUserRole> GetLatestRoleCodeAsync()
        {
            return await _dbContext.ExternalUserRoles.OrderByDescending(x => x.RoleCode).FirstOrDefaultAsync();
        }
        public async Task<Result<ExternalUserRole>> UpdateExternalUserRoleStatusAsync(ExternalUserRole userRole)
        {
             _dbContext.Update(userRole);
            await _dbContext.SaveChangesAsync();
            return userRole;
        }
        public async Task<Result<ExternalUserRole>> AddExternalUserRoleAsync(ExternalUserRole externalUser)
        {
            _dbContext.ExternalUserRoles.Add(externalUser);
            await _dbContext.SaveChangesAsync();
            return externalUser;
        }

        public async Task<ExternalUserRole> GetSystemAdminExternalRoleAsync()
        {
            return await _dbContext.ExternalUserRoles.Where(x => x.RoleCode == "EXT01").FirstOrDefaultAsync();
        }

        public async Task<List<ExternalUserRole>> GetAllExternalUserRolesBySolution(long solutionCode)
        {
            var query = await _dbContext.ExternalUserRoles
                .Where(x => x.Solution.Id == solutionCode)
                .ToListAsync();

            return query;
        }

        public async Task<ExternalUserRole> GetInitialRoleAsync(int solutionCode)
        {
            var query = await _dbContext.ExternalUserRoles
                .Include(x => x.ExternalUserRoleStatus)
                .Include(x => x.Solution)
                .Where(x => x.ExternalUserRoleName == "System Admin" && x.Solution.Id == solutionCode)
                .FirstOrDefaultAsync();

            return query;
        }
    }
}
