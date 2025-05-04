using Microsoft.AspNetCore.Mvc;
using RoipBackend.Models;
using RoipBackend.Utilities;

namespace RoipBackend.Interfaces
{
    public interface IUserService
    {
        ServiceResult<string> GetHealthCheck();

        Task<ServiceResult<List<User>>> GetAllUsersAsync(string jwt, int pageNumber, int pageSize);

        Task<ServiceResult<string>> RegisterUserAsync(User user);

        Task<ServiceResult<User>> LogInAsync(string email, string password);

        ServiceResult<string> Logout(User user);

    }
}
