using RoipBackend.Models;

namespace RoipBackend.Interfaces
{
    public interface IUserConnectionService
    {
        Task AddConnectionAsync(UserConnection connection);
        Task RemoveConnectionAsync(string connectionId);
        Task<List<UserConnection>> GetActiveConnectionsAsync();
        Task<UserConnection?> GetConnectionByUserIdAsync(int userId);
    }
}
