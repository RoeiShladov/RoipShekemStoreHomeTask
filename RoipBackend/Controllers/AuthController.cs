using Microsoft.AspNetCore.Mvc;
using RoipBackend.Dto;
using RoipBackend.Interfaces;
using RoipBackend.Models;
using RoipBackend.Services;
using System.Security.Claims;
using System.Text;

namespace RoipBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        //TODO: Add [Authorize] attribute to controller's admin functions
        //TODO: Add refresh token / cookies functionality (???)

        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        private readonly LoggerService _loggerService;

        public AuthController(IConfiguration configuration, UserService userService,LoggerService loggerService)
        {
            this._configuration = configuration;
            this._userService = userService;
            this._loggerService = loggerService;
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            //TODO: Add logger           
            var serviceResult = await _userService.GetAllUsersAsync();            
            // Extract the response object
            var response = okResult.Value as dynamic;

            // Extract the message
            string message = response?.Message;
            string eror = response?.Error;
            HandleServiceResult(response, message, error);             
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] User user)
        {
            //TODO: Add logger           
            if (!ModelState.IsValid)
                return ModelStateError(Consts.VALIDATION_FAILED_STR);

            if (!_userService.IsValidEmail(user.Email))
                return ModelStateError(Consts.INVALID_EMAIL_FORMAT_STR);

            var serviceResult = await _userService.RegisterUserAsync(user);
            // Extract the response object
            var response = okResult.Value as dynamic;

            // Extract the message
            string message = response?.Message;
            string eror = response?.Error;
            HandleServiceResult(response, message, error);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] string email, string password)
        {
            //TODO: Add logger           
            if (!ModelState.IsValid)
                return ModelStateError();

            var serviceResult = await _userService.LogIn(email, password);           

            // Extract the message
            string message = response?.Message;
            string eror = response?.Error;
            HandleServiceResult(response, message, error);
        }                    

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            //TODO: Add logger                           
            var serviceResult = _userService.Logout();

            // Extract the message
            string message = response?.Message;
            string eror = response?.Error;
            HandleServiceResult(response, message, error);
        }


        [HttpPut("edit-User")]
        public async Task<IActionResult> EditUserAsync([FromBody] User user)
        {
            //TODO: Add logger            
            if (!ModelState.IsValid)
                return ModelStateError(Consts.USER_PROPERTIES_NOT_VALID_STR);

            var serviceResult = await _userService.EditUserAsync(user);

            // Extract the message
            string message = response?.Message;
            string eror = response?.Error;
            HandleServiceResult(response, message, error);
        }
        private IActionResult ModelStateError(string message)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new { Message = message, Errors = errors });
        }        

        private IActionResult HandleServiceResult(IActionResult serviceResult, string message, string error)
        {            
            switch (serviceResult)
            {
                case OkObjectResult okResult:
                    return Ok(new
                    {
                        Message = message,
                        Data = okResult.Value
                    });

                case BadRequestObjectResult badRequestResult:
                    if(error != null)
                        return new BadRequestObjectResult(new {  message = message, Error = error });
                    return new BadRequestObjectResult(new { message = message });

                case NotFoundObjectResult:
                    if (error != null)
                        return new NotFoundObjectResult( new { message = message, Error = error });
                    return new NotFoundObjectResult(new { Message = message });

                case UnauthorizedResult:
                    if (error != null)
                        return new UnauthorizedObjectResult( new { message = message, Error = error });
                    return UnauthorizedObjectResult(new { Message = message });


                case StatusCodeResult statusCodeResult:
                    if (error != null)
                        return new StatusCode(statusCodeResult.StatusCode, new { message = message, Error = error });
                    return StatusCode(statusCodeResult.StatusCode, new { Message = message });

                case ObjectResult objectResult:
                    if (error != null)
                        return new StatusCode(objectResult.StatusCode, new { message = message, Error = error });
                    return StatusCode(objectResult.StatusCode, new { Message = message });
                
                default:
                    return StatusCode(Consts.INTERNAL_SERVER_ERROR_NUMBER, new { Message = Consts.INTERNAL_SERVER_ERROR_STR });
            }
        }
    }  
}
