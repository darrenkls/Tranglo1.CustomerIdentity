using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.SignUpCodes;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.Repositories
{
    public class SignUpCodeRepository : ISignUpCodeRepository
    {
        private readonly SignUpCodeDBContext dbContext;

        public SignUpCodeRepository(SignUpCodeDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<SignUpCode> AddSignUpCodesAsync(SignUpCode signUpCodeInfo)
        {
            this.dbContext.SignUpCodes.Add(signUpCodeInfo);
            await this.dbContext.SaveChangesAsync();
            return signUpCodeInfo;
        }

        public async Task<IEnumerable<SignUpAccountStatus>> GetSignUpCodeAccountStatusAsync()
        {
            var query = this.dbContext.SignUpAccountStatuses;

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<LeadsOrigin>> GetSignUpCodeLeadsOriginAsync()
        {
            var query = this.dbContext.LeadsOrigins;

            return await query.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<SignUpCode> GetSignUpCodesAsync(long id)
        {
            return await dbContext.SignUpCodes
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<SignUpCode> GetSignUpCodesAsync(string signUpCode)
        {
            return await dbContext.SignUpCodes
                            .Where(x => x.Code == signUpCode)
                            .Include(x=>x.Status)
                            .FirstOrDefaultAsync();
        }

        public async Task<SignUpCode> GetFilteredSignUpCodeByCompanyNameAsync(string companyName, long solutionCode)
        { 
            // get signup code list with Active/Used status only 
            return await dbContext.SignUpCodes
                .Where(x => x.CompanyName == companyName 
                && (x.ExpireAt > DateTime.UtcNow || x.Status == SignUpAccountStatus.Used)
                && x.SolutionCode == solutionCode)
                .Include(x => x.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(); 
        }

        public async Task<SignUpCode> GetActiveSignUpCodeByCompanyNameAsync(string companyName)
        {
            // get signup code list with Active status only
            return await dbContext.SignUpCodes
                .Where(x => x.CompanyName == companyName && x.ExpireAt > DateTime.UtcNow )
                .Include(x => x.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<SignUpCode> UpdateSignUpCodesAsync(SignUpCode updatedSigUpCodeInfo)
        {
            this.dbContext.SignUpCodes.Update(updatedSigUpCodeInfo);
            await this.dbContext.SaveChangesAsync();
            return updatedSigUpCodeInfo;
        }
    }
}

