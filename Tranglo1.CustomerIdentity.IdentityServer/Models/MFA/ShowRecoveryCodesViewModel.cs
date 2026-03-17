using Microsoft.AspNetCore.Mvc;

namespace Tranglo1.CustomerIdentity.IdentityServer.Models.MFA
{
    public class ShowRecoveryCodesViewModel
    {
        [TempData]
        public string[] RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
    }
}
