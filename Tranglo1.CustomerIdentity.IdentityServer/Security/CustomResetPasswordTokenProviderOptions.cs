using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Security
{
    public class CustomResetPasswordTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public CustomResetPasswordTokenProviderOptions()
        {
            Name = "CustomResetPasswordDataProtectorTokenProvider";
            TokenLifespan = TimeSpan.FromHours(2);
        }
    }
}
