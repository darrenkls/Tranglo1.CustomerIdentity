using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.Domain.Repositories
{
    public interface ITrangloRoleRepository
    {
        Task<TrangloRole> GetLatestRoleByDept(int departmentId);
        TrangloRole GetTrangloRoleByCode(string roleCode);
        Task<TrangloRole> GetTrangloRoleByCodeAsync(string roleCode);
        Task<List<string>> GetTrangloEntityByLoginId(string loginId);
        Task<TrangloEntity> GetTrangloEntityAsync(string trangloEntityCode);
        Task<List<TrangloRole>> GetTrangloRoles();
        Task<List<TrangloRole>> GetRolesInDepartment(int deptId);
        Task<Result<TrangloRole>> UpdateTrangloRoleStatus(TrangloRole trangloRole);
        Task<Result<TrangloRole>> UpdateTrangloRole(TrangloRole trangloRole);
        Task<Result<TrangloRole>> AddTrangloRole(TrangloRole trangloRole);
        Task<bool> UserHasTrangloEntity(TrangloStaff trangloStaff, string trangloEntityCode);
    }
}
