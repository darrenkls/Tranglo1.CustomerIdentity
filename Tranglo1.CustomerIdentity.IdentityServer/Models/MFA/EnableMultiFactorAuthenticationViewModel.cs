using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tranglo1.CustomerIdentity.IdentityServer.Models.MFA
{
    public class EnableMultiFactorAuthenticationViewModel
    {
        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }
        public string CurrentScreen { get; set; }
        public List<string> RecoveryCode { get; set; }
    }
}
