using CSharpFunctionalExtensions;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class CustomerUserBusinessProfile : Entity
    {
        public int UserId { get; set; }
        public int BusinessProfileCode { get; set; }
        public Environment Environment { get; set; }
        public CompanyUserAccountStatus CompanyUserAccountStatus { get; set; }
        public CompanyUserBlockStatus CompanyUserBlockStatus { get; set; }

        protected CustomerUserBusinessProfile()
        {
        }

        public CustomerUserBusinessProfile(int userId, int businessProfileCode)
        {
            this.UserId = userId;
            this.BusinessProfileCode = businessProfileCode;
            this.Environment = Environment.Staging;
            this.CompanyUserAccountStatus = CompanyUserAccountStatus.Active;
            this.CompanyUserBlockStatus = CompanyUserBlockStatus.Unblock;
        }

        public void SetCompanyUserBlockStatus(CompanyUserBlockStatus companyUserBlockStatus)
        {
            this.CompanyUserBlockStatus = companyUserBlockStatus;
        }

        public void SetCompanyUserAccountStatus(CompanyUserAccountStatus companyUserAccountStatus)
        {
            if (this.CompanyUserAccountStatus != companyUserAccountStatus)
            {
                this.CompanyUserAccountStatus = companyUserAccountStatus;
            }
        }
    }
}
