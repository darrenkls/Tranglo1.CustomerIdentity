using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Providers
{
    public class SignalrUserLoginProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext context)
        {
            var id = ClaimsPrincipalExtensions.GetSubjectId(context.User);
            var stringId = id == null ? string.Empty : id.ToString();

            return stringId;
        }
    }
}
