namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class CustomerUserRegistration
    {
        public long CustomerUserRegistrationId { get; set; }
        public string LoginId { get; set; }
        public string CompanyName { get; set; }
        public string SignUpCode { get; set; }
        public int? SolutionCode { get; set; }
        public int? CustomerTypeCode { get; set; }
        public int BusinessProfileCode { get; set; }
        public PartnerRegistrationLeadsOrigin? PartnerRegistrationLeadsOrigin { get; set; }
        public string? OtherPartnerRegistrationLeadsOrigin { get; set; }

        public CustomerUserRegistration(Email loginId, CompanyName companyName, int? solutionCode = null, int? customerTypeCode = null, PartnerRegistrationLeadsOrigin? leadsOrigin = null, string? otherLeadsOrigin = null)
        {
            this.LoginId = loginId.Value;
            this.CompanyName = companyName.Value;
            this.SolutionCode = solutionCode;
            this.CustomerTypeCode = customerTypeCode;
            this.PartnerRegistrationLeadsOrigin = leadsOrigin;
            this.OtherPartnerRegistrationLeadsOrigin = otherLeadsOrigin;
        }

        public CustomerUserRegistration(Email loginId, CompanyName companyName, int businessProfile, int solutionCode)
        {
            this.LoginId = loginId.Value;
            this.CompanyName = companyName.Value;
            this.BusinessProfileCode = businessProfile;
            this.SolutionCode = solutionCode;
        }

        public CustomerUserRegistration(Email loginId, string signUpCode, string companyName)
        {
            this.LoginId = loginId.Value;
            this.SignUpCode = signUpCode;
            this.CompanyName = companyName;
        }

        public CustomerUserRegistration(Email loginId, string signUpCode, string companyName, int businessProfile, PartnerRegistrationLeadsOrigin? leadsOrigin = null, string? otherLeadsOrigin = null)
        {
            this.LoginId = loginId.Value;
            this.SignUpCode = signUpCode;
            this.CompanyName = companyName;
            this.BusinessProfileCode = businessProfile;
            this.PartnerRegistrationLeadsOrigin = leadsOrigin;
            this.OtherPartnerRegistrationLeadsOrigin = otherLeadsOrigin;
        }

        private CustomerUserRegistration()
        {

        }
    }
}