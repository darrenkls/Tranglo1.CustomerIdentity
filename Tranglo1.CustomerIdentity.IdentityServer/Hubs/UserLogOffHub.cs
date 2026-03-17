using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Services.SignalR;

namespace Tranglo1.CustomerIdentity.IdentityServer.Hubs
{
    //NOTE:
    // Only store SignalR connection and group handling in the hub
    // Sending of SignalR messages to client side should be handled in a service class (e.g SignalRMessageService)

    public class UserLogOffHub : Hub
    {
        private readonly ILogger<UserLogOffHub> _logger = null;

        public UserLogOffHub(ILogger<UserLogOffHub> logger)
        {
            _logger = logger;
        }

        // user to join group based on businessProfileCode to be invoke by client
        public async Task JoinGroup(int businessProfileCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, businessProfileCode.ToString());
        }

        // user to leave group on disconnect based on businessProfileCode to be invoke by client
        public async Task RemoveFromGroup(int businessProfileCode)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, businessProfileCode.ToString());
        }

        // a lifecyce hook that invoke on client connect
        // can add extra stuff for some event handling
        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"{Context.UserIdentifier} connected as {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        // a lifecyce hook that invoke on client disconnect
        // can add extra stuff for some event handling
        public override Task OnDisconnectedAsync(Exception exception)
        {
            // await RemoveFromGroup()
            _logger.LogInformation($"{Context.UserIdentifier} disconnected as {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
