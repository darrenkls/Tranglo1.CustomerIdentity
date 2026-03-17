using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tranglo1.CustomerIdentity.IdentityServer.Security
{
    public class InvitationTokenProvider<TUser>
                                           : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public InvitationTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<InvitationTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger)
                                              : base(dataProtectionProvider, options, logger)
        {

        }
    }
}
