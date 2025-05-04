using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using RoipBackend.Interfaces;
using RoipBackend.Models;
using RoipBackend.Services;
using RoipBackend.Utilities;
using System.ComponentModel.DataAnnotations;

namespace RoipBackend.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly LoggerService _loggerService;

        public UsersController(UserService userService, LoggerService loggerService, JwtAuthService jwtAuthService)
        {
            _userService = userService;
            _loggerService = loggerService;
        }

        [HttpGet(C.HEALTH_CHECK_API_STR)]
        [AllowAnonymous]
        public async Task<IActionResult> GetJWTUserDetailsAsync(
            [Required(ErrorMessage = C.JWT_MODEL_STATE_INVALID_STR)] string jwt)
        {
            if (!ModelState.IsValid)
            {
                await _loggerService.LogWarningAsync(C.MODEL_STATE_VALIDATION_FAILED_STR);
                return ModelStateError(C.MODEL_STATE_VALIDATION_FAILED_STR);
            }

            ServiceResult<AuthenticatedUserDTO> result = await _userService.GetJWTUserResolverAsync(jwt);
            if (result.Success)
            {
                return Ok(new
                {
                    result.Success,                   
                    result.StatusCode,
                    result.Data
                });
            }

            return new ObjectResult(new { Message = result.Message, Error = result.Error, StatusCode = result.StatusCode });
        }

        [HttpGet(C.HEALTH_CHECK_API_STR)]
        [AllowAnonymous]
        public async Task<IActionResult> GetHealthCheckAsync()
        {
            ServiceResult<string> result = _userService.GetHealthCheck();
            if (result.Success)
            {
                return Ok(new
                {
                    result.Message,
                    result.StatusCode,                    
                });
            }

            await _loggerService.LogFatalAsync(string.Empty, C.HEALTH_CHECK_FAILED_STR);
            return new ObjectResult(new { Message = result.Message, Error = result.Error, StatusCode = result.StatusCode });
        }


        [HttpGet(C.GET_ALL_USERS_API_STR)]
        [Authorize(Roles = C.ADMIN_STR)]        
        public async Task<IActionResult> GetAllUsersAsync(
            [Required(ErrorMessage = C.JWT_MODEL_STATE_INVALID_STR)] string jwt,
            int pageNumber,
            int pageSize)
        {            
            ServiceResult<List<User>> result = await _userService.GetAllUsersAsync(jwt, pageNumber, pageSize);
            if (result.Success)
            {
                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                    result.Data
                });
            }           
            return HandleStatusCode(result.StatusCode, result.Message, result.Error);            
        }        


        [HttpPost(C.REGISTER_API_STR)]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(User user)
        {
            if (!ModelState.IsValid)
            {
                await _loggerService.LogWarningAsync(C.MODEL_STATE_INVALID_STR);
                return ModelStateError(C.MODEL_STATE_VALIDATION_FAILED_STR);
            }

            ServiceResult<string> result = await _userService.RegisterUserAsync(user);
            if (result.Success)
            {
                await _loggerService.LogInfoAsync(result.Message);
                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                });
            }

            return HandleStatusCode(result.StatusCode, result.Message, result.Error);
        }


        [HttpPost(C.LOGIN_API_STR)]
        [AllowAnonymous]
        public async Task<IActionResult> LogInAsync(
            [StringLength(100, ErrorMessage = C.USER_EMAIL_MODEL_STATE_INVALID_STR)] string email,
            [StringLength(16, ErrorMessage = C.USER_PASSWORD_MODEL_STATE_INVALID_STR)] string password)
            {
                if (!ModelState.IsValid)
                {
                    await _loggerService.LogWarningAsync(C.MODEL_STATE_VALIDATION_FAILED_STR);
                    return ModelStateError(C.MODEL_STATE_VALIDATION_FAILED_STR);
                }

                ServiceResult<User> result = await _userService.LogInAsync(email, password);

                if (result.Success)
                {                
                    return Ok(new
                    {
                        result.Message,
                        result.StatusCode,
                        result.Data,
                        result.RoipShekemStoreJWT
                    });
                }

                return HandleStatusCode(result.StatusCode, result.Message, result.Error);
            }


        [HttpPost(C.LOGOUT_API_STR)]
        [Authorize(Roles = $"{C.ADMIN_STR},{C.CUSTOMER_STR}")]
        public async Task<IActionResult> LogoutAsync(User user)
        {
            if (!ModelState.IsValid)
            {
                await _loggerService.LogWarningAsync(C.MODEL_STATE_INVALID_STR);
                return ModelStateError(C.MODEL_STATE_VALIDATION_FAILED_STR);
            }

            ServiceResult<string> result = _userService.Logout(user);                       
            return Ok(new
            {
                result.Message,
                result.StatusCode,
                //Empty string (The JWT in the explorer is not nessecery anymore and will be removed)
                result.RoipShekemStoreJWT 
            });            
        }


        private BadRequestObjectResult ModelStateError(string message)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new { Message = message, Errors = errors, StatusCode = StatusCodes.Status400BadRequest });
        }

        private IActionResult HandleStatusCode(int statusCode, string message, string error)
        {
            switch (statusCode)
            {
                case StatusCodes.Status400BadRequest:
                    return BadRequest(new { Message = message, Error = error, StatusCode = statusCode });

                case StatusCodes.Status401Unauthorized:
                    return Unauthorized(new { Message = message, Error = error, StatusCode = statusCode });

                case StatusCodes.Status403Forbidden:
                    return StatusCode(StatusCodes.Status403Forbidden, new { Message = message, Error = error, StatusCode = statusCode });

                case StatusCodes.Status404NotFound:
                    return NotFound(new { Message = message, Error = error, StatusCode = statusCode });

                case StatusCodes.Status408RequestTimeout:
                    return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = message, Error = error, StatusCode = statusCode });

                case StatusCodes.Status409Conflict:
                    return Conflict(new { Message = message, Error = error, StatusCode = statusCode });

                case StatusCodes.Status500InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = message, Error = error, StatusCode = statusCode });

                default:
                    return new ObjectResult(new { Message = message, Error = error, StatusCode = statusCode });
            }
        }
    }
}