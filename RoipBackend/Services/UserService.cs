using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoipBackend.Interfaces;
using RoipBackend.Models;
using RoipBackend.Services;
using RoipBackend.Utilities;

namespace RoipBackend.Services
{
    public class UserService : IUserService
    {
        //TODO: Add Loggers to all functions
        private readonly AppDbContext _DBcontext;
        private readonly LoggerService _loggerService;
        private readonly JwtHelper _jwtHelper;

        public UserService(AppDbContext context, LoggerService loggerService, JwtHelper jwtHelper)
        {
            _DBcontext = context;
            _loggerService = loggerService;
            _DBcontext.Database.SetCommandTimeout(Consts.DB_REQUEST_TIMEOUT);
            _jwtHelper = jwtHelper;
        }

        public async Task<IActionResult> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            try
            {
                //Get users when changing the page number
                var result = await _DBcontext.Users
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (result != null && result.Count > 0)
                {
                    return new OkObjectResult(new { Message = Consts.USERS_RETRIEVE_SUCCESS_STR, Data = result })
                    {
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                else
                {
                    return new NotFoundObjectResult(new { Message = Consts.NO_USERS_FOUND_STR, Error = Consts.NO_USERS_FOUND_DESC_STR })
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
            }
            catch (OperationCanceledException e)
            {
                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            catch (Exception e)
            {
                return new ObjectResult(new { Message = Consts.FAILED_RETRIEVING_USERS_STR, Error = Consts.INTERNAL_SERVER_ERROR_STR })
                {
                    StatusCode = Consts.INTERNAL_SERVER_ERROR_NUMBER
                };
            }
        }

        public async Task<IActionResult> RegisterUserAsync(User user)
        {
            try
            {
                // RowVersion is included for concurrency check
                _DBcontext.Entry(user).Property(u => u.RowVersion).OriginalValue = user.RowVersion;
                var existingUser = await _DBcontext.Users
                                .FirstOrDefaultAsync(u => u.Email == user.Email || u.Id == user.Id);

                if (existingUser != null)
                {
                    return new ConflictObjectResult(new { Message = Consts.EMAIL_ALREADY_EXISTS_STR, Error = Consts.EMAIL_ALREADY_EXISTS_DESC_STR })
                    {
                        StatusCode = StatusCodes.Status409Conflict  
                    };
                }
                // Hashing the password, and adding the user to the database
                user.Password = HashPassword(user.Password);
                _DBcontext.Users.Add(user);
                await _DBcontext.SaveChangesAsync();
                return new OkObjectResult(new { Message = Consts.USER_REGISTERED_SUCCESSFULLY_STR })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (DbUpdateConcurrencyException e)
            {
                return new ConflictObjectResult(new { Message = Consts.CONCURRENCY_ERROR_STR, Error = Consts.CONCURRENCY_ERROR_DESC_STR })
                {
                    StatusCode = StatusCodes.Status409Conflict
                };
            }
            catch (DbUpdateException e)
            {
                // Handle database update exceptions (e.g., unique constraint violations)
                return new ConflictObjectResult(new { Message = Consts.DATABASE_UPDATE_ERROR_STR, Error = Consts.DATABASE_UPDATE_ERROR_DESC_STR })
                {
                    StatusCode = StatusCodes.Status409Conflict
                };
            }
            catch (OperationCanceledException e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.DATABASE_CONNECTION_TIMEOUT_STR);
                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            catch (Exception e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.FAILED_RETRIEVING_USERS);
                return new ObjectResult(new { Message = Consts.REGISTRATION_FAILED_DESC_STR, Error = Consts.INTERNAL_SERVER_ERROR_STR })
                {
                    StatusCode = Consts.INTERNAL_SERVER_ERROR_NUMBER
                };
            }
        }

        public async Task<IActionResult> LogInAsync(string email, string password)
        {
            try
            {
                var user = await _DBcontext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    //_loggerService.LogFailedLogin(email); // Abstracted logging  
                    return new NotFoundObjectResult(new {Message = Consts.NO_EMAIL_FOUND_STR,
                        Error = $"{Consts.EMAIL_NOT_FOUND1_STR} {email} {Consts.EMAIL_NOT_FOUND2_STR}"})
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if (!VerifyPassword(password, user.Password))
                {
                    //_loggerService.LogFailedLogin(email); // incorrect password logger  
                    return new UnauthorizedObjectResult(new { Message = Consts.WRONG_PASSWORD_STR, Error = Consts.WRONG_PASSWORD_DESC_STR })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
               }                
                
                string new_JWT = _jwtHelper.GenerateToken(user.Id.ToString(), user.Role, Consts.JWT_EXPIRATION_TIME);
                return new OkObjectResult(new { Message = Consts.USER_LOGGED_IN_SUCCESSFULLY_STR, RoipShekemStoreJWT = new_JWT })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }            
            catch (OperationCanceledException e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.DATABASE_CONNECTION_TIMEOUT_STR);  
                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            catch (Exception e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.FAILED_LOGIN_STR);  
                return new ObjectResult(new { Message = Consts.FAILED_LOGIN_DESC_STR, Error = Consts.INTERNAL_SERVER_ERROR_STR })
                {
                    StatusCode = Consts.INTERNAL_SERVER_ERROR_NUMBER
                };
            }
        }        

        public IActionResult Logout()
        {                      
            return new OkObjectResult(new { Message = Consts.LOGOUT_SUCCESSFULLY_STR, RoipShekemStoreJWT = string.Empty })
            {
                StatusCode = StatusCodes.Status200OK
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
