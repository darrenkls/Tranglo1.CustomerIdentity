using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Hubs;

namespace Tranglo1.CustomerIdentity.IdentityServer.Services.SignalR
{
    public class SignalRMessageService
    {
        // NOTE:
        // Sending of SignalR messages to the client side is added here
        // SignalR connections and groups will be handled in the hub
        // Invoke these methods from controller/command classes. Can be used by client side to prompt alerts to users

        private readonly IHubContext<UserLogOffHub> _userLogOffHub;

        public SignalRMessageService(IHubContext<UserLogOffHub> userLogOffHub)
        {
            _userLogOffHub = userLogOffHub;
        }

        // Alert users tied to BusinessProfileCode to log off/refresh page (Redo Business Declaration)
        public async Task RedoBusinessDeclarationLogOffAlert(int businessProfileCode)
        {
            await _userLogOffHub.Clients.Group(businessProfileCode.ToString()).SendAsync("redoBusinessDeclarationLogOffAlert");
        }

        // Alert users tied to BusinessProfileCode to log off/refresh page (Non Redo Business Declaration)
        public async Task NonRedoBusinessDeclarationLogOffAlert(int businessProfileCode)
        {
            await _userLogOffHub.Clients.Group(businessProfileCode.ToString()).SendAsync("nonRedoBusinessDeclarationLogOffAlert");
        }
    }
}