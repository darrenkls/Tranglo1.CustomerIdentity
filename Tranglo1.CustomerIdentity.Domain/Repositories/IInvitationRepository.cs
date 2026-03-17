using System.Collections.Generic;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.Domain.Repositories
{
    public interface IInvitationRepository
    {
        Task<IReadOnlyList<Invitation>> GetInvitationsAsync(Specification<Invitation> filters);

        Invitation AddInvitations(Invitation invitation);

        //Invitation AddBusinessProfiles(Invitation invitation);

        Task<int> SaveChangesAsync(Invitation invitation);
    }
}
