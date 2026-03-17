using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerUser
{
    public class InviteUserInputDTO
    {
        //[Required(ErrorMessage = "User environment code is required")]
        public int UserEnvironmentCode { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(150)]
        public string InviteeFullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(150)]
        [DataType(DataType.EmailAddress)]
        public string InviteeEmail { get; set; }

        [Required(ErrorMessage = "Invitee role code is required")]
        public List<string> InviteeRoleCodeList { get; set; }

        public string Timezone { get; set; }
        public int SolutionCode { get; set; }
    }
}