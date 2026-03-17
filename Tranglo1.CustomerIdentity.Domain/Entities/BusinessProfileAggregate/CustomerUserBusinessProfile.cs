using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class CustomerUserBusinessProfile : Entity
    {
        public CustomerUser CustomerUser { get; private set; }
        public int UserId { get; set; }
        public BusinessProfile BusinessProfile { get; private set; }
        public int BusinessProfileCode { get; set; }
        //public AccountStatus AccountStatus { get; private set; }
        public Environment Environment { get; set; }
        public CompanyUserAccountStatus CompanyUserAccountStatus { get; set; }
        public CompanyUserBlockStatus CompanyUserBlockStatus { get; set; }



        private CustomerUserBusinessProfile()
        {

        }

        public CustomerUserBusinessProfile(CustomerUser customerUser, BusinessProfile businessProfile)
        {
            this.CustomerUser = customerUser;
            this.BusinessProfile = businessProfile;
            this.UserId = customerUser.Id;
            this.BusinessProfileCode = businessProfile.Id;
            this.Environment = Environment.Staging;
            this.CompanyUserAccountStatus = CompanyUserAccountStatus.Active;
            this.CompanyUserBlockStatus = CompanyUserBlockStatus.Unblock;
            //this.AccountStatus = customerUser.AccountStatus;
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
