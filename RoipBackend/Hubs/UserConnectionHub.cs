using Microsoft.AspNetCore.SignalR;
using RoipBackend.Interfaces;
using RoipBackend.Models;
using RoipBackend.Services;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Security.Claims;

namespace RoipBackend.Hubs
{ 
    public class UserConnectionHub : Hub
    {
        private static ConcurrentDictionary<string, SignalRUserDTO> ConnectedUsers = new();
        private readonly LoggerService _loggerService;

        // Constructor to inject LoggerService
        public UserConnectionHub(LoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                //The Hub can generate a connectionId for each open session.
                //In general, the same user(email) can have multiple
                //connections(via multiple devices or browsers opened tabs to the system).        
                ConnectedUsers.TryAdd(Context.ConnectionId, new SignalRUserDTO
                {
                    ConnectionId = Context.ConnectionId,
                    Username = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "",
                    Email = Context.User?.FindFirst(ClaimTypes.Email)?.Value ?? "",
                    Role = Context.User?.FindFirst(ClaimTypes.Role)?.Value ?? C.ANONYMOUS_STR
                });
                await Clients.All.SendAsync(C.HUB_LIVE_BROADCAST_ACTION_STR, ConnectedUsers.ToList());
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                string userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "";
                string userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value ?? "";
                string role = Context.User?.FindFirst(ClaimTypes.Role)?.Value ?? C.ANONYMOUS_STR;
                string connectedUser = $"{C.NAME_STR}: {userName}, {C.EMAIL_STR}: ({userEmail}), {C.ROLE_STR}: ({role})";
                await _loggerService.LogErrorAsync(ex.Message, $"{C.ERROR_CONNECTING_HUB_STR} {connectedUser}");
            }            
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                string connectionId = Context.ConnectionId;
                ConnectedUsers.TryRemove(connectionId, out _);
                await Clients.All.SendAsync(C.HUB_LIVE_BROADCAST_ACTION_STR, ConnectedUsers.ToList());
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                string userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "";
                string userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value ?? "";
                string role = Context.User?.FindFirst(ClaimTypes.Role)?.Value ?? C.ANONYMOUS_STR;
                string connectedUser = $"{C.NAME_STR}: {userName}, {C.EMAIL_STR}: ({userEmail}), {C.ROLE_STR}: ({role})";
                await _loggerService.LogErrorAsync(ex.Message, $"{C.ERROR_DISCONNECTING_HUB_STR} {connectedUser}");
            }
        }

        public async Task FetchAuthenticatedUser(string email, string name, string role)
        {
            try
            {
                string connectionId = Context.ConnectionId;
                if (ConnectedUsers.ContainsKey(connectionId))
                {
                    ConnectedUsers[connectionId] = new SignalRUserDTO
                    {
                        ConnectionId = connectionId,
                        Username = name,
                        Email = email,
                        Role = role
                    };
                    await Clients.All.SendAsync(C.HUB_LIVE_BROADCAST_ACTION_STR, ConnectedUsers.ToList());
                }               
            }
            catch (Exception ex)
            {                
                string connectedUser = $"{C.NAME_STR}: {name}, {C.EMAIL_STR}: ({email}), {C.ROLE_STR}: ({role})";
                await _loggerService.LogErrorAsync(ex.Message, $"{C.ERROR_FETCHING_AUTHENTICATED_USER_STR} {connectedUser}");
            }
        }        
    }
}