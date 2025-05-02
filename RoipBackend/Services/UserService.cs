using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoipBackend.Interfaces;
using RoipBackend.Models;
using RoipBackend.Services;

namespace RoipBackend.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _DBcontext;
        private readonly LoggerService _loggerService;
        private static readonly HashSet<string> TokenBlacklist = new();

        //TODO: Add loggers to class
        public UserService(AppDbContext context, LoggerService loggerService)
        {
            _DBcontext = context;
            _loggerService = loggerService;
            _DBcontext.Database.SetCommandTimeout(Consts.DB_REQUEST_TIMEOUT);
        }

        public async Task<IActionResult> GetAllUsersAsync()//int pageNumber, int pageSize)
        {
            try
            {
                var result = await _DBcontext.Users.ToListAsync();

                if (result != null && result.Count > 0)
                {
                    return new OkObjectResult(new { Message = Consts.USERS_RETRIEVE_SUCCESS_STR, Data = result })
                    {
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                else
                {
                    return new NotFoundObjectResult(new { Message = Consts.NO_USERS_FOUND_STR, Error = Consts.NO_USERS_FOUND_DESC_STR });
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
                return new BadRequestObjectResult(new { Message = Consts.FAILED_RETRIEVING_USERS_STR, Error = Consts.FAILED_RETRIEVING_USERS_DESC_STR });
            }
        }

        public async Task<IActionResult> RegisterUserAsync(User user)
        {
            try
            {
                // RowVersion is included for concurrency check, hashing the password, and adding the user to the database
                _DBcontext.Entry(user).Property(u => u.RowVersion).OriginalValue = user.RowVersion;
                user.Password = HashPassword(user.Password);                
                _DBcontext.Users.Add(user);
                var existingUser = await _DBcontext.Users
                                .FirstOrDefaultAsync(u => u.Email == user.Email || u.Username == user.Username);

                if (existingUser != null)
                {
                    return new ConflictObjectResult(new
                    {
                        Message = Consts.EMAIL_ALREADY_EXISTS_STR,
                        Error = Consts.EMAIL_ALREADY_EXISTS_STR
                    });
                }
                await _DBcontext.SaveChangesAsync();
                return new OkObjectResult(new { Message = Consts.USER_REGISTERED_SUCCESSFULLY_STR })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (DbUpdateConcurrencyException e)
            {
                return new ConflictObjectResult(new
                {
                    Message = Consts.CONCURRENCY_ERROR_STR,
                    Error = Consts.CONCURRENCY_ERROR_DESC_STR
                });
            }
            catch (DbUpdateException e)
            {
                // Handle database update exceptions (e.g., unique constraint violations)
                return new ConflictObjectResult(new { Message = Consts.DATABASE_UPDATE_ERROR_STR, Error = Consts.DATABASE_UPDATE_ERROR_DESC_STR });
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
                return new BadRequestObjectResult(new { Message = Consts.REGISTRATION_FAILED_STR, Error = Consts.REGISTRATION_FAILED_DESC_STR });
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
                    return new NotFoundObjectResult(new
                    {
                        Message = Consts.NO_EMAIL_FOUND_STR,
                        Error = $"{Consts.EMAIL_NOT_FOUND1_STR} {email} {Consts.EMAIL_NOT_FOUND2_STR}"
                    })
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

                _DBcontext.Entry(user).Property(u => u.RowVersion).OriginalValue = user.RowVersion;
                // TODO: generate authentication cookies / JWT tokens to this user, return them to the client in the message, add resolve in client side.

                return new OkObjectResult(new { Message = Consts.USER_LOGGED_IN_SUCCESSFULLY_STR })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (DbUpdateConcurrencyException e)
            {
                return new ConflictObjectResult(new
                {
                    Message = Consts.CONCURRENCY_ERROR_STR,
                    Error = Consts.CONCURRENCY_ERROR_DESC_STR
                });
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
                return new BadRequestObjectResult(new { Message = Consts.FAILED_LOGIN_STR, Error = Consts.FAILED_LOGIN_DESC_STR });
            }
        }        

        public IActionResult Logout()
        {           
            try
            {
                return new OkObjectResult(new { Message = Consts.LOGOUT_SUCCESSFULLY_STR })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (DbUpdateConcurrencyException e)
            {
                return new ConflictObjectResult(new
                {
                    Message = Consts.CONCURRENCY_ERROR_STR,
                    Error = Consts.CONCURRENCY_ERROR_DESC_STR
                });
            }
            catch (Exception e)
            {
                // Log the exception if necessary
                return new BadRequestObjectResult(new { Message = Consts.LOGOUT_FAILED_STR, Error = e.Message });
            }
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
