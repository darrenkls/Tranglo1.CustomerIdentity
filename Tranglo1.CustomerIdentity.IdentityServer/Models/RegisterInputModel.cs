using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.IdentityServer.Models
{
    public class RegisterInputModel : IValidatableObject
    {
        [MaxLength(150)]
        [Display(Name = "CompanyName")]
        //[RegularExpression(@"^[a-zA-Z0-9\s]*$")]
        public string CompanyName { get; set; }

        [MaxLength(150)]
        [Display(Name = "Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(150)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [StringLength(128, MinimumLength = 12)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
                    ErrorMessage = "Password must contain at least 12 characters, 1 lower case, 1 upper case, 1 numeric and 1 symbol")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        
        [Compare("Password", ErrorMessage = "Confirm password doesn't match")]
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Solution")]
        public int? SolutionCode { get; set; }

        [MaxLength(150)]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Registry Code only allow alphanumeric with uppercase letter")]
        [Display(Name = "RegistryCode")]
        public string RegistryCode { get; set; }

        //[Required(ErrorMessage = "User Type is required")]
        //public int UserTypeCode { get; set; }

        public string CountryISO2 { get; set; }

        public int PartnerRegistrationLeadsOriginCode { get; set; }

        public string OtherLeadsOrigin { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the Terms and Conditions!")]
        public bool IsTermsAndConditionReadAgreed { get; set; }
        public string RecaptchaToken { get; set; }

        [Display(Name = "Customer Type")]
        public int? CustomerTypeCode { get; set; }

        public bool HasSignUpCode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            //if ( this.CustomerTypeCode is null && this.SolutionCode == Solution.Business.Id )
            //{
            //    results.Add(new ValidationResult("Customer Type is required for solution " + Solution.Business.Name));
            //}
            if (!this.HasSignUpCode && this.CountryISO2 is null)
            {
                results.Add(new ValidationResult("Country is required for self sign up"));
            }

            //If no sign up code flow
            if( !this.HasSignUpCode )
            {
                //Company Name required
                if( String.IsNullOrEmpty(this.CompanyName))
                {
                    results.Add(new ValidationResult("Company is required"));
                }
                //Email required
                if (String.IsNullOrEmpty(this.Email))
                {
                    results.Add(new ValidationResult("Email is required"));
                }

                var validationContextEmail = new ValidationContext(this.Email);
                var validationResultsEmail = new System.Collections.Generic.List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(this.Email, validationContextEmail, validationResultsEmail, true);

                if (!isValid)
                {
                    results.Add(new ValidationResult("Email format invalid"));
                }

                if ( this.CustomerTypeCode != CustomerType.Individual.Id)
                {
                    //Full Name required
                    if (String.IsNullOrEmpty(this.FullName))
                    {
                        results.Add(new ValidationResult("Full Name is required"));
                    }
                }
            }

            return results;
        }
    
    
    }
}
