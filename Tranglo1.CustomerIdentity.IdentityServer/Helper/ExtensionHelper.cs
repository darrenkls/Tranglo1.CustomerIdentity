using System.Runtime.CompilerServices;

namespace Tranglo1.CustomerIdentity.IdentityServer.Helper
{
    public static class ExtensionHelper
    {
        public static string GetMethodName([CallerMemberName] string callerMemberName = "") => callerMemberName;
    }
}
