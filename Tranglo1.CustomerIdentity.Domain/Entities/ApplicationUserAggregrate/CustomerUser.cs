using System;
using System.Collections.Generic;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Events;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class CustomerUser : ApplicationUser
    {
        protected CustomerUser() : base()
        {

        }

        //public CustomerUser(
        //    FullName fullName, Email email, string passwordHash, 
        //    Solution solution, UserType userType, Country country)
        //    : base(fullName, email)
        //{
        //    this.PasswordHash = passwordHash;
        //    this.Solution = solution;
        //    this.UserType = userType;
        //    this.Country = country;
        //    this.AccountStatus = AccountStatus.PendingActivation;

        //    CustomerUserRegisteredEvent customerUserRegisteredEvent =
        //                new CustomerUserRegisteredEvent(email.Value, fullName.Value, DateTime.Now);
        //    base.AddDomainEvent(customerUserRegisteredEvent);
        //}

        public virtual bool EmailConfirmed { get; protected set; }
        public virtual string PasswordHash { get; set; }
        //public Solution Solution { get; protected set; }
        //public long SolutionCode { get; set; }
        //public UserType UserType { get; protected set; }
        //public long UserTypeCode { get; set; }
        //public string CountryISO2 { get; set; }
        //public Country Country { get; protected set; }
        public CountryMeta CountryMeta { get; protected set; }
        public virtual bool IsTPNUser { get; protected set; }


        public void ConfirmInviteeEmail()
        {
            if (this.EmailConfirmed == false)
            {
                this.EmailConfirmed = true;
                this.SetAccountStatus(AccountStatus.Active);
            }
        }
        public void setIsTPNUser(bool value)
        {
            IsTPNUser = value;
        }

        public void ConfirmEmail(int? solutionCode, bool isMultipleSolution)
        {
            if (this.EmailConfirmed == false)
            {
                this.ConfirmInviteeEmail();
                //Register event on confirmation of email
                //if (this.Solution == Solution.Connect)
                base.AddDomainEvent(new CustomerUserEmailVerifiedEvent(this.Email.Value, solutionCode, isMultipleSolution));
            }
        }

        public override void SetEmail(Email email)
        {
            //When customer user change email, this means that their login id will be changed too.
            base.SetEmail(email);
            base.LoginId = email.Value;
        }

        internal static CustomerUser Create(FullName fullName, Email email, string passwordHash, CountryMeta countryMeta, string timezone)
        {
            var customer = new CustomerUser
            {
                PasswordHash = passwordHash,
                //Solution = solution,
                //UserType = userType,
                //Country = country,
                CountryMeta = countryMeta,
                AccountStatusCode = AccountStatus.PendingActivation.Id
                //AccountStatus = AccountStatus.PendingActivation
            };

            customer.SetEmail(email);
            customer.SetName(fullName);
            customer.SetTimezone(timezone);

            return customer;

        }

        public static CustomerUser Register(FullName fullName, Email email, string passwordHash, CountryMeta countryMeta, int? solutionCode, bool isMultipleSolutions)
        {
            var customerUser = Create(fullName, email, passwordHash, countryMeta, null);

            CustomerUserRegisteredEvent customerUserRegisteredEvent =
                new CustomerUserRegisteredEvent(email.Value, fullName.Value, DateTime.UtcNow, solutionCode, isMultipleSolutions);

            customerUser.AddDomainEvent(customerUserRegisteredEvent);

            return customerUser;
        }
    }
}
