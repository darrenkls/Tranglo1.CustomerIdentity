using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Security
{
    public class CustomResetPasswordTokenProvider<TUser>
                                           : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public CustomResetPasswordTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<CustomResetPasswordTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger)
                                              : base(dataProtectionProvider, options, logger)
        {

        }
    }
}
