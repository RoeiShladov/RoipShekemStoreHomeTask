using Microsoft.AspNetCore.Mvc;
using RoipBackend.Models;

namespace RoipBackend.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> GetAllUsersAsync();
        Task<User?> GetUserByEmailAsync(string email);
        Task<IActionResult> LogInAsync(string username, string password);
        Task<IActionResult> EditUserAsync(User user);

        Task<IActionResult> RegisterUserAsync(User user);

        IActionResult Logout();
    }
}
