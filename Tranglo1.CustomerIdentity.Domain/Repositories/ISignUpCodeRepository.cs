using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.SignUpCodes;

namespace Tranglo1.CustomerIdentity.Domain.Repositories
{
    public interface ISignUpCodeRepository
    {
        Task<IEnumerable<LeadsOrigin>> GetSignUpCodeLeadsOriginAsync();
        Task<IEnumerable<SignUpAccountStatus>> GetSignUpCodeAccountStatusAsync();
        Task<SignUpCode> AddSignUpCodesAsync(SignUpCode signUpCodeInfo);
        Task<SignUpCode> UpdateSignUpCodesAsync(SignUpCode updatedSigUpCodeInfo);
        Task<SignUpCode> GetSignUpCodesAsync(long id);
        Task<SignUpCode> GetSignUpCodesAsync(string registryCode);
        Task<SignUpCode> GetFilteredSignUpCodeByCompanyNameAsync(string companyName, long solutionCode);
        Task<SignUpCode> GetActiveSignUpCodeByCompanyNameAsync(string companyName);
    }
}
