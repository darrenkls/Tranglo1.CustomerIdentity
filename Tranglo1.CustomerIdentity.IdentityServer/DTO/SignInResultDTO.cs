using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO
{
    public class SignInResultDTO
    {
        public SignInResult SignInResult { set; get; }
        public bool IsResetPassword { set; get; }
        public string ResetPasswordToken { set; get; }
        public AuthenticationType AuthenticationType { get; set; }
    }
}
