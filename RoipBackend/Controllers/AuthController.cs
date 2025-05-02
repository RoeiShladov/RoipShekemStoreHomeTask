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
        //TODO: Add refresh token / cookies functionality (???)

        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        private readonly LoggerService _loggerService;

        public AuthController(IConfiguration configuration, UserService userService, LoggerService loggerService)
        {
            _configuration = configuration;
            _userService = userService;
            _loggerService = loggerService;
        }

        [HttpGet("get-all-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            //TODO: Add logger           
            IActionResult serviceResult = await _userService.GetAllUsersAsync();                        
            return HandleServiceResult(serviceResult);             
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> RegisterAsync([FromBody] User user)
        {
            //TODO: Add logger           
            if (!ModelState.IsValid)
                return ModelStateError(Consts.VALIDATION_FAILED_STR);

            IActionResult serviceResult = await _userService.RegisterUserAsync(user);            
            return HandleServiceResult(serviceResult);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] string email, string password)
        {
            //TODO: Add logger           
            if (!ModelState.IsValid)
                return ModelStateError(Consts.VALIDATION_FAILED_STR);

            IActionResult serviceResult = await _userService.LogInAsync(email, password);                       
            return HandleServiceResult(serviceResult);
        }                    

        [HttpPost("logout")]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult Logout()
        {
            //TODO: Add logger                           
            IActionResult serviceResult = _userService.Logout();
            return HandleServiceResult(serviceResult);
        }
        
        private BadRequestObjectResult ModelStateError(string message)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return new BadRequestObjectResult(new { Message = message, Errors = errors });
        }

        private static ActionResult HandleServiceResult(IActionResult serviceResult)
        {
            return ServiceResultHandler.HandleServiceResult(serviceResult);
        }
    }  
}
