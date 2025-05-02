using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoipBackend.Dto;
using RoipBackend.Interfaces;
using RoipBackend.Models;
using RoipBackend.Services;
using RoipBackend.Utilities;
using System.Security.Cryptography;

namespace RoipBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        //TODO: Add Loggers to all functions
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        private readonly LoggerService _loggerService;
        private readonly JwtHelper _jwtHelper;
        private readonly IUserConnectionService _userConnectionService;

        public AuthController(IConfiguration configuration, UserService userService, LoggerService loggerService, IUserConnectionService userConnectionService, JwtHelper jwt)
        {
            _configuration = configuration;
            _userService = userService;
            _loggerService = loggerService;
            _userConnectionService = userConnectionService;
            _jwtHelper = jwt;
        }


        [HttpGet("get-all-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersAsync(string jwt, int pageNumber, int pageSize)
        {
            IActionResult jwt_ValidationResult = _jwtHelper.JwtCheck(jwt);
            if (jwt_ValidationResult is UnauthorizedObjectResult)
                return jwt_ValidationResult;

            return await _userService.GetAllUsersAsync(pageNumber, pageSize);
        }
        

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(User user)
        {
            //TODO: Add logger           
            if (!ModelState.IsValid)
                return ModelStateError(Consts.VALIDATION_FAILED_STR);

            return await _userService.RegisterUserAsync(user);            
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(string email, string password)
        {
            if (!ModelState.IsValid)
                return ModelStateError(Consts.VALIDATION_FAILED_STR);

            var loginResult = await _userService.LogInAsync(email, password);
            if (loginResult is OkObjectResult okResult)
            {
                // Handle user connection logic here
                User? user = okResult.Value as User;
                if (user == null || string.IsNullOrEmpty(user.Email))
                {
                    return new BadRequestObjectResult(new { Message = Consts.FAILED_LOGIN_STR, Error = Consts.FAILED_LOGIN_DESC_STR })
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                HandleUserConnection(user);
            }
            return loginResult;
        }

        [HttpPost("logout")]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult Logout()
        {
            var result = _userService.Logout();
            //Keeping the 'if(result is OkObjectResult okResult)' logic for potential future need,
            //in case 'Logout()' function's logic in UserService will expand & the return type will be unknown.
            if (result is OkObjectResult okResult)
            {
                // Handle user disconnection logic here.
                User? user = okResult.Value as User;
                if (user == null || string.IsNullOrEmpty(user.Email))
                {
                    return new BadRequestObjectResult(new { Message = Consts.LOGOUT_FAILED_STR, Error = Consts.LOGOUT_FAILED_DESC_STR })
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };                    
                }
                HandleUserDisconnection(user);
            }
            return result;
        }

        private BadRequestObjectResult ModelStateError(string message)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return new BadRequestObjectResult(new { Message = message, Errors = errors })
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }        
        
        private string GenerateSessionId()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        private void HandleUserConnection(User user)
        {
            _userConnectionService.AddConnectionAsync(new UserConnection
            {
                ConnectionId = GenerateSessionId(),
                Email = user.Email,
                ConnectedAt = DateTime.UtcNow
            });
        }

        private void HandleUserDisconnection(User user)
        {
            _userConnectionService.AddConnectionAsync(new UserConnection
            {
                ConnectionId = GenerateSessionId(),
                Email = user.Email,
                ConnectedAt = DateTime.UtcNow
            });
        }
    }
}
