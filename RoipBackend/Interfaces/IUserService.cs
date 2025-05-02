using Microsoft.AspNetCore.Mvc;
using RoipBackend.Models;

namespace RoipBackend.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<IActionResult> LogInAsync(string username, string password);
        Task<IActionResult> RegisterUserAsync(User user);

        IActionResult Logout();
    }
}
