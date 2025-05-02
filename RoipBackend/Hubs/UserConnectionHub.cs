using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace RoipBackend.Hubs
{
    public class UserConnectionHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> ConnectedUsers = new();

        public override async Task OnConnectedAsync()
        {            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {        
            await base.OnDisconnectedAsync(exception);
        }
    }
}