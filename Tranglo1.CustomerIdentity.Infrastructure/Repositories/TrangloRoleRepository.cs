using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.Repositories
{
    public class TrangloRoleRepository : ITrangloRoleRepository
    {
        private readonly TrangloRoleDbContext _dbContext;
        private readonly IApplicationUserRepository _applicationUserRepository;
        public TrangloRoleRepository(TrangloRoleDbContext dbContext, IApplicationUserRepository applicationUserRepository)
        {
            _dbContext = dbContext;
            _applicationUserRepository = applicationUserRepository;
        }

        public TrangloRole GetTrangloRoleByCode(string roleCode)
        {
            return _dbContext.TrangloRoles.Include(e => e.RoleStatus).FirstOrDefault(x => x.Id.Trim().ToLower() == roleCode.Trim().ToLower());
        }

        public async Task<TrangloRole> GetTrangloRoleByCodeAsync(string roleCode)
        {
            return await _dbContext.TrangloRoles.Include(e => e.RoleStatus).FirstOrDefaultAsync(x => x.Id.Trim().ToLower() == roleCode.Trim().ToLower());
        }

        public async Task<List<string>> GetTrangloEntityByLoginId(string loginId)
        {
            return await _dbContext.TrangloStaffEntityAssignments
                .Where(x => x.LoginId == loginId)
                .Select(x => x.TrangloEntity)
                .ToListAsync();
        }

        public async Task<TrangloEntity> GetTrangloEntityAsync(string trangloEntityCode)
        {
            return await _dbContext.TrangloEntities
                .Where(x => x.TrangloEntityCode == trangloEntityCode)
                .FirstOrDefaultAsync();
        }


        public async Task<TrangloRole> GetLatestRoleByDept(int deptId)
        {
            var result = await _dbContext.TrangloRoles.Where(x => x.TrangloDepartment.Id == deptId).OrderByDescending(a => a.Id)
              .FirstOrDefaultAsync();

            return result;
        }

        public Task<List<TrangloRole>> GetTrangloRoles()
        {
            return _dbContext.TrangloRoles.ToListAsync();
        }

        public async Task<List<TrangloRole>> GetRolesInDepartment(int deptId)
        {
            if (deptId != 0)
            {
                var query = _dbContext.TrangloRoles.Where(x => x.TrangloDepartment.Id == deptId).ToListAsync();
                return await query;
            }

            return await Task.FromResult<List<TrangloRole>>(null);
        }
        public async Task<Result<TrangloRole>> UpdateTrangloRoleStatus(TrangloRole trangloRole)
        {
            _dbContext.Update(trangloRole);
            await _dbContext.SaveChangesAsync();

            return trangloRole;

        }
        public async Task<Result<TrangloRole>> UpdateTrangloRole(TrangloRole trangloRole)
        {
            _dbContext.Update(trangloRole);
            await _dbContext.SaveChangesAsync();

            return trangloRole;

        }
        public async Task<Result<TrangloRole>> AddTrangloRole(TrangloRole trangloRole)
        {
            _dbContext.Entry(trangloRole).State = EntityState.Added;
            await _dbContext.SaveChangesAsync();

            return trangloRole;

        }

        public async Task<bool> UserHasTrangloEntity(
            TrangloStaff trangloStaff, string trangloEntityCode)
        {
            var trangloStaffEntity = await _applicationUserRepository.GetTrangloStaffEntityAssignmentById(trangloStaff.LoginId);
            if (trangloStaffEntity != null)
            {
                //var check = trangloStaffEntity.Where(x => x.TrangloEntity == trangloEntityByPartner);
                foreach (var item in trangloStaffEntity)
                {
                    if (item.TrangloEntity.Trim().ToLower() == trangloEntityCode.Trim().ToLower())
                    {
                        return true;
                    }
                }
            }
            else if (trangloEntityCode == null)
            {
                return true;
            }
            return false;
        }
    }
}
