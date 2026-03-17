using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.Domain.Repositories
{
    public interface IMetaRepository
    {
        Task<List<Solution>> GetAllSolutionAsync();
        Task<List<UserType>> GetAllUserType();
        Task<List<AccountStatus>> GetAllAccountStatus();

    }
}
