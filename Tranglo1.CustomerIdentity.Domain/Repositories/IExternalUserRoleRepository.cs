using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.ExternalUserRoleAggregate;

namespace Tranglo1.CustomerIdentity.Domain.Repositories
{
    public interface IExternalUserRoleRepository
    {
        public ExternalUserRole[] GetAllExternalUserRoles();
        Task<ExternalUserRole> GetSystemAdminExternalRoleAsync();
        Task<ExternalUserRole> GetExternalRoleByRoleCodeAsync(string roleCode);
        Task<ExternalUserRole> GetLatestRoleCodeAsync();
        Task<Result<ExternalUserRole>> UpdateExternalUserRoleStatusAsync(ExternalUserRole userRole);
        Task<Result<ExternalUserRole>> AddExternalUserRoleAsync(ExternalUserRole userRole);
        Task<List<ExternalUserRole>> GetAllExternalUserRolesBySolution(long solutionCode);
        Task<ExternalUserRole> GetInitialRoleAsync(int solutionCode);
    }
}