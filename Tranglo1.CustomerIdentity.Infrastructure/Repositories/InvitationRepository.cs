using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.Repositories
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly ApplicationUserDbContext dbContext;

        public InvitationRepository(ApplicationUserDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Invitation>> GetInvitationsAsync(Specification<Invitation> filters)
        {
            var query = this.dbContext.Invitations
                .Where(filters.ToExpression());

            return await query.ToListAsync();
        }

        //public Invitation AddInvitations(Invitation invitation)
        //{
        //    var _invitation = this.dbContext.Add(invitation).Entity;

        //    this.dbContext.SaveChanges();

        //    return _invitation;
        //}

        public Invitation AddInvitations(Invitation invitation)
        {
            this.dbContext.Invitations.Add(invitation);
            this.dbContext.SaveChanges();
            return invitation;
        }

        public async Task<int> SaveChangesAsync(Invitation invitation)
        {
            foreach (var reference in dbContext.Entry(invitation).References)
            {
                reference.TargetEntry.State = EntityState.Modified;
            }

            return await this.dbContext.SaveChangesAsync();
        }
    }
}
