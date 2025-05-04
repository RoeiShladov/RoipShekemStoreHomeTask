using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace RoipBackend.Hubs
{ 
    public class UserConnectionHub : Hub
    {
        private static ConcurrentDictionary<string, string> ConnectedUsers = new();       

        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            string userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? C.ANONYMOUS_STR;
            string userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value ?? "";

            ConnectedUsers.TryAdd(connectionId, C.ANONYMOUS_STR);
            await Clients.All.SendAsync(C.HUB_LIVE_BROADCAST_STR, ConnectedUsers.ToList());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;
            ConnectedUsers.TryRemove(connectionId, out _);
            await Clients.All.SendAsync(C.HUB_LIVE_BROADCAST_STR, ConnectedUsers.ToList());
            await base.OnDisconnectedAsync(exception);
        }

        public async Task FetchAndUpdateLoggedInUsers(Dictionary<string, string> userMappings)
        {
            foreach (var mapping in userMappings)
            {
                string connectionId = mapping.Key;
                string userId = mapping.Value;

                if (ConnectedUsers.ContainsKey(connectionId))
                {
                    ConnectedUsers[connectionId] = userId;
                }
            }

            await Clients.All.SendAsync(C.HUB_LIVE_BROADCAST_STR, ConnectedUsers.ToList());
        }
        public async Task UpdateUserToAuthenticated(string connectionId, string userId)
        {
            if (ConnectedUsers.ContainsKey(connectionId))
            {
                ConnectedUsers[connectionId] = userId;
                await Clients.All.SendAsync(C.HUB_LIVE_BROADCAST_STR, ConnectedUsers.ToList());
            }
        }


        //public static ConcurrentDictionary<string, string> GetConnectedUsers()
        //{
        //    if (ConnectedUsers == null || ConnectedUsers.IsEmpty)
        //    {
        //        return new ConcurrentDictionary<string, string>();
        //    }
        //    return ConnectedUsers;
        //}

        //public async Task GetConnectedUsersForAdmin()
        //{
        //    await Clients.Caller.SendAsync(C.HUB_LIVE_ON_DEMAND_BROADCAST_STR, ConnectedUsers.ToList());
        //}
    }
}