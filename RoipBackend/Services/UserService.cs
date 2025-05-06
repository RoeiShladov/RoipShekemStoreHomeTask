using Microsoft.EntityFrameworkCore;
using RoipBackend.Interfaces;
using RoipBackend.Models;
using RoipBackend.Utilities;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;

namespace RoipBackend.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _DBcontext;
        private readonly LoggerService _loggerService;
        private readonly JwtAuthService _jwtAuthService;

        public UserService(AppDbContext context, LoggerService loggerService, JwtAuthService jwtAuthService)
        {
            _DBcontext = context;
            _loggerService = loggerService;
            _jwtAuthService = jwtAuthService;
            _DBcontext.Database.SetCommandTimeout(C.DB_REQUEST_TIMEOUT);
        }

        public async Task<ServiceResult<AuthenticatedUserDTO>> GetJWTUserResolverAsync(string jwt)
        {
            try
            {
                var claimsPrincipal = await _jwtAuthService.GetJwtClaims(jwt);

                if (claimsPrincipal == null)
                {
                    string friendlyDescribtion = $"{C.JWT_INVALID_STR}. {C.JWT_INVALID_DESC_STR}";
                    await _loggerService.LogErrorAsync(string.Empty, friendlyDescribtion);
                    return new ServiceResult<AuthenticatedUserDTO>
                    {
                        Success = false,
                        Message = C.JWT_INVALID_STR,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Error = C.JWT_INVALID_DESC_STR
                    };
                }

                var nameClaim = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
                var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
                var roleClaim = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;
                AuthenticatedUserDTO dTO = new AuthenticatedUserDTO
                {
                    Username = nameClaim,
                    Email = emailClaim,
                    Role = roleClaim
                };

                return new ServiceResult<AuthenticatedUserDTO>
                {
                    Success = true,
                    Message = C.JWT_DETAILS_RETRIEVED_SUCCESSFULLY_STR,
                    StatusCode = StatusCodes.Status200OK,
                    Data = dTO
                };
            }
            catch (Exception e)
            {
                string friendlyDescribtion = $"{C.JWT_CONTENT_CHECK_FAILED_STR}. {C.JWT_CONTENT_CHECK_FAILED_DESC_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<AuthenticatedUserDTO>
                {
                    Success = false,
                    Message = C.JWT_CONTENT_CHECK_FAILED_STR,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = C.INTERNAL_SERVER_ERROR_STR
                };
            }
        }

        public ServiceResult<string> GetHealthCheck()
        {
            return new ServiceResult<string>
            {
                Success = true,
                Message = C.HEALTH_CHECK_SUCCEEDED_STR,
                StatusCode = StatusCodes.Status200OK,
            };
        }

        public async Task<ServiceResult<List<User>>> GetAllUsersAsync(string jwt, int pageNumber, int pageSize)
        {
            try
            {
                ServiceResult<string> validationResult = await _jwtAuthService.JwtContentCheck(jwt, C.ADMIN_STR);
                if (!validationResult.Success)
                {
                    await _loggerService.LogWarningAsync($"{validationResult.Message}. {validationResult.Error}. {validationResult.StatusCode}");
                    return new ServiceResult<List<User>>
                    {
                        Message = validationResult.Message,
                        Error = validationResult.Error,
                        StatusCode = validationResult.StatusCode
                    };
                }

                //Client side validation should be fine but it is good to have server side validation as well.
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return new ServiceResult<List<User>>
                    {
                        Success = false,
                        Message = C.INVALID_PAGINATION_PARAMETERS_STR,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                // Get users when changing the page number
                List<User> result = await _DBcontext.Users
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (result != null && result.Count > 0)
                {
                    //await _loggerService.LogInfoAsync($"{C.USERS_RETRIEVE_SUCCESS_STR}. {result.Count} {C.USERS_COUNT_STR}");
                    return new ServiceResult<List<User>>
                    {
                        Success = true,
                        Message = C.USERS_RETRIEVE_SUCCESS_STR,
                        StatusCode = StatusCodes.Status200OK,
                        Data = result
                    };
                }
                else
                {
                    string friendlyDescribtion = $"{C.NO_USERS_FOUND_STR}. {C.NO_USERS_FOUND_DESC_STR}";
                    await _loggerService.LogErrorAsync(string.Empty, friendlyDescribtion);
                    return new ServiceResult<List<User>>
                    {
                        Success = false,
                        Message = C.NO_USERS_FOUND_STR,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = C.NO_USERS_FOUND_DESC_STR
                    };
                }
            }
            catch (OperationCanceledException e)
            {
                string friendlyDescribtion = $"{C.REQUEST_TIME_OUT_STR}. {C.DATABASE_CONNECTION_TIMEOUT_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<List<User>>
                {
                    Success = false,
                    Message = C.REQUEST_TIME_OUT_STR,
                    StatusCode = StatusCodes.Status408RequestTimeout,
                    Error = C.DATABASE_CONNECTION_TIMEOUT_STR
                };
            }
            catch (Exception e)
            {
                string friendlyDescribtion = $"{C.FAILED_RETRIEVING_USERS_STR}. {C.FAILED_RETRIEVING_USERS_DESC_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<List<User>>
                {
                    Success = false,
                    Message = C.FAILED_RETRIEVING_USERS_DESC_STR,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = C.INTERNAL_SERVER_ERROR_STR
                };
            }
        }

        public async Task<ServiceResult<string>> RegisterUserAsync(User user)
        {
            try
            {
                var existingUser = await _DBcontext.Users
                                .FirstOrDefaultAsync(u => u.Email == user.Email || u.Id == user.Id);

                if (existingUser != null)
                {
                    string friendlyDescribtion = $"{C.EMAIL_ALREADY_EXISTS_STR}. {C.EMAIL_ALREADY_EXISTS_DESC_STR}";
                    await _loggerService.LogErrorAsync(string.Empty, friendlyDescribtion);
                    return new ServiceResult<string>
                    {
                        Success = false,
                        Message = C.EMAIL_ALREADY_EXISTS_STR,
                        StatusCode = StatusCodes.Status409Conflict,
                        Error = C.EMAIL_ALREADY_EXISTS_DESC_STR
                    };
                }

                // Hashing the password, and adding the user to the database
                user.Password = HashPassword(user.Password);
                _DBcontext.Users.Add(user);
                await _DBcontext.SaveChangesAsync();

                //await _loggerService.LogInfoAsync($"{C.USER_REGISTERED_SUCCESSFULLY_STR}: {user.Email}", user.Id);
                return new ServiceResult<string>
                {
                    Success = true,
                    Message = C.USER_REGISTERED_SUCCESSFULLY_STR,
                    StatusCode = StatusCodes.Status200OK,
                };
            }
            catch (DbUpdateException e)
            {
                string friendlyDescribtion = $"{C.DATABASE_UPDATE_ERROR_STR}. {C.DATABASE_UPDATE_ERROR_DESC_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<string>
                {
                    Success = false,
                    Message = C.DATABASE_UPDATE_ERROR_STR,
                    StatusCode = StatusCodes.Status409Conflict,
                    Error = C.DATABASE_UPDATE_ERROR_DESC_STR
                };
            }
            catch (OperationCanceledException e)
            {
                string friendlyDescribtion = $"{C.REQUEST_TIME_OUT_STR}. {C.DATABASE_CONNECTION_TIMEOUT_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<string>
                {
                    Success = false,
                    Message = C.REQUEST_TIME_OUT_STR,
                    StatusCode = StatusCodes.Status408RequestTimeout,
                    Error = C.DATABASE_CONNECTION_TIMEOUT_STR
                };
            }
            catch (Exception e)
            {
                string friendlyDescribtion = $"{C.REGISTRATION_FAILED_STR}. {C.REGISTRATION_FAILED_DESC_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<string>
                {
                    Success = false,
                    Message = C.REGISTRATION_FAILED_DESC_STR,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = C.INTERNAL_SERVER_ERROR_STR
                };
            }
        }

        public async Task<ServiceResult<User>> LogInAsync(string email, string password)
        {
            try
            {
                var user = await _DBcontext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    string error = $"{C.EMAIL_NOT_FOUND1_STR} {email} {C.EMAIL_NOT_FOUND2_STR}";
                    await _loggerService.LogWarningAsync($"{C.FAILED_LOGIN_STR}. {error}");
                    return new ServiceResult<User>
                    {
                        Success = false,
                        Message = C.NO_EMAIL_FOUND_STR,
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = error,
                    };
                }

                //Comparing the password with the hashed password in the database
                if (!VerifyPassword(password, user.Password))
                {
                    await _loggerService.LogWarningAsync($"{C.LOGIN_PASSWORD_FAILED_STR} {email}");
                    return new ServiceResult<User>
                    {
                        Success = false,
                        Message = C.WRONG_PASSWORD_STR,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Error = C.WRONG_PASSWORD_DESC_STR,
                    };
                }

                //User is found and password is correct, Generate JWT token and return it to the user
                string newJwt = await _jwtAuthService.GenerateJWT(user.Id, user.Username, user.Email, user.Role, C.JWT_EXPIRATION_TIME);
                if (string.IsNullOrEmpty(newJwt))
                {
                    string friendlyDescribtion = $"{C.JWT_GENERATION_FAILED_STR}. {C.JWT_GENERATION_FAILED_DESC_STR}";
                    await _loggerService.LogErrorAsync(string.Empty, C.JWT_GENERATION_FAILED_STR);
                    return new ServiceResult<User>
                    {
                        Success = false,
                        Message = C.JWT_GENERATION_FAILED_STR,
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Error = C.JWT_GENERATION_FAILED_DESC_STR
                    };
                }
                //await _loggerService.LogInfoAsync($"{C.JWT_GENERATED_SUCCESSFULLY_STR}: {newJwt}", user.Id);
                //await _loggerService.LogInfoAsync($"{C.USER_LOGGED_IN_SUCCESSFULLY_STR}: {email}");
                return new ServiceResult<User>
                {
                    Success = true,
                    Message = C.USER_LOGGED_IN_SUCCESSFULLY_STR,
                    StatusCode = StatusCodes.Status200OK,
                    RoipShekemStoreJWT = newJwt,
                    Data = user
                };
            }
            catch (OperationCanceledException e)
            {
                string friendlyDescribtion = $"{C.REQUEST_TIME_OUT_STR}. {C.DATABASE_CONNECTION_TIMEOUT_STR}";
                await _loggerService.LogErrorAsync(e.Message, friendlyDescribtion);
                return new ServiceResult<User>
                {
                    Success = false,
                    Message = C.REQUEST_TIME_OUT_STR,
                    Error = C.DATABASE_CONNECTION_TIMEOUT_STR,
                    StatusCode = StatusCodes.Status408RequestTimeout,
                };
            }
            catch (Exception e)
            {
                await _loggerService.LogErrorAsync(e.Message, C.FAILED_LOGIN_STR);
                return new ServiceResult<User>
                {
                    Success = false,
                    Message = C.FAILED_LOGIN_DESC_STR,
                    Error = C.INTERNAL_SERVER_ERROR_STR,
                    StatusCode = StatusCodes.Status500InternalServerError,
                };
            }
        }

        public ServiceResult<string> Logout(User user)
        {
            return new ServiceResult<string>
            {
                Success = true,
                Message = C.LOGOUT_SUCCESSFULLY_STR,
                StatusCode = StatusCodes.Status200OK,
                RoipShekemStoreJWT = string.Empty
            };
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }        
    }
}