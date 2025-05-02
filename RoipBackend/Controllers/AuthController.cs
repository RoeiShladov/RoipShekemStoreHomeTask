using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoipBackend.Dto;
using RoipBackend.Models;
using RoipBackend.Services;
using RoipBackend.Utilities;

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

        public AuthController(IConfiguration configuration, UserService userService, LoggerService loggerService, JwtHelper jwt)
        {
            _configuration = configuration;
            _userService = userService;
            _loggerService = loggerService;
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

            return await _userService.LogInAsync(email, password);                       
        }                    

        [HttpPost("logout")]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult Logout()
        {            
            return _userService.Logout();            
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
    }
}
