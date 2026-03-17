using Microsoft.AspNetCore.Identity;
using System;

namespace Tranglo1.CustomerIdentity.IdentityServer.Security
{
    public class InvitationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public InvitationTokenProviderOptions()
        {
            Name = "InvitationTokenProvider";
            TokenLifespan = TimeSpan.FromHours(24);
        }
    }
}
