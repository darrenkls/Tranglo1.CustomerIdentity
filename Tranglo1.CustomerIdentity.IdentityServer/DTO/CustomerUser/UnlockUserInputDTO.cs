using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerUser
{
    public class UnlockUserInputDTO
    {
        [Required]
        public string Email { get; set; }
    }
}
