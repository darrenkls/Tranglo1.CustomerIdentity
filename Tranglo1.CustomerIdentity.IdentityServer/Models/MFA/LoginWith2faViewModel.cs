using System.ComponentModel.DataAnnotations;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;

namespace Tranglo1.CustomerIdentity.IdentityServer.Models.MFA
{
    public class LoginWith2faViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Code { get; set; }
        public long AuthenticationType { get; set; }
        public string Error { get; set; }
    }
}
