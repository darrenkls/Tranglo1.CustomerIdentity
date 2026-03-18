using System.Collections.Generic;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.Domain.DomainServices
{
    public interface IStaffEntityQueryService
    {
        Task<List<TrangloStaffEntityAssignment>> GetTrangloStaffEntityAssignmentById(string loginId);
    }
}
