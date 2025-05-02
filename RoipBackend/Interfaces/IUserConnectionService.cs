using Microsoft.AspNetCore.Mvc;
using RoipBackend.Models;

namespace RoipBackend.Interfaces
{
    public interface IUserConnectionService
    {
        Task<IActionResult> AddConnectionAsync(UserConnection connection);
        Task<IActionResult> RemoveConnectionAsync(string connectionId);
        Task<IActionResult> GetActiveConnectionsAsync();

        //Task<IActionResult> GetConnectionByEmailAsync(int email);
    }
}
