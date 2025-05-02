using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RoipBackend.Hubs;
using RoipBackend.Interfaces;
using RoipBackend.Models;

namespace RoipBackend.Services
{
    public class UserConnectionService : IUserConnectionService
    {
        private readonly IHubContext<UserConnectionHub> _hubContext;
        private static List<UserConnection> _activeConnections;
        private static readonly object _lock = new object();


        public UserConnectionService(IHubContext<UserConnectionHub> hubContext)
        {
            _hubContext = hubContext;
            _activeConnections = new List<UserConnection>();
        }

        [Authorize(Roles = "Customer")]
        public async void AddConnectionAsync(UserConnection connection)
        {
            lock (_lock)
            {
                _activeConnections.Add(connection);
            }
            await _hubContext.Clients.All.SendAsync("UpdateConnections", _activeConnections);
        }

        [Authorize(Roles = "Customer")]
        public async void RemoveConnectionAsync(UserConnection connection)
        {
            lock (_lock)
            {
                var c = _activeConnections.FirstOrDefault(connection);
                if (c != null)
                {
                    _activeConnections.Remove(c);
                }
            }
            await _hubContext.Clients.All.SendAsync("UpdateConnections", _activeConnections);
        }

        [Authorize(Roles = "Admin")]
        public List<UserConnection> GetActiveConnections()
        {
            return _activeConnections;
        }
    }
}
