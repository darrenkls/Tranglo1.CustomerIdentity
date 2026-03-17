using System.ComponentModel.DataAnnotations;

namespace Tranglo1.CustomerIdentity.IdentityServer.Models
{
    public class InviteUserModel
    {
        //[Required(ErrorMessage = "User environment code is required")]
        public int UserEnvironmentCode { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(300)]
        public string InviteeFullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(320)]
        [DataType(DataType.EmailAddress)]
        public string InviteeEmail { get; set; }

        [Required(ErrorMessage = "Invitee role code is required")]
        public long InviteeRoleCode { get; set; }
    }
}
