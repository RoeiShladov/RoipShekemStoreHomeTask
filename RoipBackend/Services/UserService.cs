using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoipBackend.Interfaces;
using RoipBackend.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RoipBackend.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _DBcontext;
        private readonly LoggerService _loggerService;

        public UserService(AppDbContext context, LoggerService loggerService)
        {
            _DBcontext = context;
            _DBcontext.Database.SetCommandTimeout(Consts.DB_REQUEST_TIMEOUT); // Timeout in seconds
            _loggerService = loggerService;
        }

        public async Task<IActionResult> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            // Add pagination to this function.
            try
            {
                //var result = await ExecuteDbOperationAsync(() =>
                //    _DBcontext.Users
                //    .Skip((pageNumber - 1) * pageSize)
                //    .Take(pageSize)
                //    .ToListAsync()
                //);

                var result = await _DBcontext.Users.ToListAsync();

                if (result != null && result.Any())
                {
                    //await _loggerService.LogInfoAsync(Consts.USER_RETRIEVE_SUCCESS_STR, Consts.USERS_RETRIEVE_SUCCESS_DESC_STR);
                    return new OkObjectResult(new { Message = Consts.USERS_RETRIEVE_SUCCESS_STR, Users = Users = result })
                    {
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                else
                {
                    //await _loggerService.LogWarningAsync(Consts.NO_USERS_FOUND_STR);
                    return new NotFoundObjectResult(new { Message = Consts.NO_USERS_FOUND_STR, Error = Consts.NO_USERS_FOUND_DESC_STR }));
                }
            }
            catch (Exception e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.FAILED_RETRIEVING_USERS);
                return new BadRequestObjectResult(new { Message = Consts.FAILED_RETRIEVING_USERS, Error = Consts.FAILED_RETRIEVING_USERS_DESC });
            }
            catch (OperationCanceledException e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.DATABASE_CONNECTION_TIMEOUT_STR);

                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            finally
            {
                //await _loggerService.LogDebugAsync(Consts.GET_ALL_USERS_FINALLY_MSG_STR, Consts.GET_ALL_USERS_FINALLY_MSG_DETAILS_STR);
            }
        }

        public async Task<IActionResult> RegisterUserAsync(User user)
        {
            try
            {
                user.Password = HashPassword(user.Password);
                _DBcontext.Users.Add(user);
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { Message = Consts.USER_REGISTERED_SUCCESSFULLY_STR })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.FAILED_RETRIEVING_USERS);
                return new BadRequestObjectResult(new { Message = Consts.REGISTRATION_FAILED_STR, Error = Consts.REGISTRATION_FAILED_DESC_STR });
            }
            catch (OperationCanceledException e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.DATABASE_CONNECTION_TIMEOUT_STR);

                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            finally
            {
                //await _loggerService.LogDebugAsync(Consts.REGISTER_USER_FINALLY_MSG_STR, Consts.REGISTER_USER_FINALLY_MSG_DESC_STR);
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
                    return new NotFoundObjectResult(new { Message = Consts.NO_EMAIL_FOUND_STR, 
                        Error = $"{Consts.EMAIL_NOT_FOUND1_STR} {email} {Consts.EMAIL_NOT_FOUND2_STR}" })
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

                // TODO: generate authentication cookies / JWT tokens to this user, return them to the client in the message, add resolve in client side.
                return new OkObjectResult(new { Message = Consts.USER_LOGGED_IN_SUCCESSFULLY_STR })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.FAILED_LOGIN_STR);  
                return new BadRequestObjectResult(new { Message = Consts.FAILED_LOGIN_STR, Error = Consts.FAILED_LOGIN_DESC_STR });
            }
            catch (OperationCanceledException e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.DATABASE_CONNECTION_TIMEOUT_STR);  

                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            finally
            {
                //await _loggerService.LogDebugAsync(Consts.LOGIN_ASYNC_FINALLY_STR, Consts.LOGIN_ASYNC_FINALLY_MSG_STR);              
            }
        }

        public async Task<IActionResult> EditUserAsync(User user)
        {
            try
            {
                var result = await _userService.GetUserByEmailAsync(user.Email);
                var response = okResult.Value as dynamic;
                User existingUser = response?.User;
                string message = response?.Message;
                string error = response?.Error;
                StatusCodes statusCode = response?.StatusCode;

                // There already existing user with the same email, with different ID.
                if (existingUser != null && user.Id != existingUser.Id)
                {
                    //_loggerService.LogInfoAsync(Consts.EMAIL_ALREADY_EXISTS_STR); // Abstracted logging  
                    return new ObjectResult(new { Message = Consts.EMAIL_ALREADY_EXISTS_STR})
                    {
                        StatusCode = StatusCodes.Status409Conflict
                    };
                }

                if (existingUser != null)
                {
                    existingUser.Username = user.Username;
                    existingUser.Password = user.Password;
                    existingUser.Email = user.Email;
                    existingUser.PhoneNumber = user.PhoneNumber;
                    existingUser.Address = user.Address;

                    // Save changes to the database  
                    await _DBcontext.SaveChangesAsync();
                    return new OkObjectResult(new { Message = Consts.USER_UPDATED_SUCCESSFULLY_STR })
                    {
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                
                if(statusCode = StatusCodes.Status404NotFound)
                {
                    //_loggerService.LogErrorAsync(Consts.USER_NOT_FOUND_STR); // Abstracted logging  
                    return new NotFoundObjectResult(new { Message = message, Error = error })
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                return new BadRequestObjectResult(new { Message = message, Eror = eror })
                {
                    StatusCode = statusCode
                };
            }
            catch (Exception e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.FAILED_LOGIN_STR);  
                return new BadRequestObjectResult(new { Message = Consts.FAILED_EDIT_STR, Error = Consts.FAILED_EDIT_DESC_STR });
            }
            catch (OperationCanceledException e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.DATABASE_CONNECTION_TIMEOUT_STR);  

                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }
            finally
            {
                //await _loggerService.LogDebugAsync(Consts.EDIT_USER_FINALLY_STR, Consts.EDIT_USER_FINALLY_MSG_STR);                          
            }
        }

        public IActionResult Logout()
        {
            //TODO: invalidate tokens or clear cookies, switch to login page in client
            return new OkObjectResult(new { Message = Consts.LOGOUT_SUCCESSFULLY_STR })
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<ActionResult> GetUserByEmailAsync(string email)
        {
            try
            {
                var result = await _DBcontext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (result == null)
                {
                    //_loggerService.LogErrorAsync($"{Consts.EMAIL_NOT_FOUND1_STR} {email} {Consts.EMAIL_NOT_FOUND2_STR}");
                    return new NotFoundObjectResult(new { Message = $"{Consts.EMAIL_NOT_FOUND1_STR} {email} {Consts.EMAIL_NOT_FOUND2_STR}",
                    Error = Consts.NO_EMAIL_FOUND_STR })
                    {                        {
                        StatusCode = StatusCodes.Status404NotFound
                    };                   
                }
                return new OkObjectResult(new { User = result })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception e)
            {
               //await _loggerService.LogErrorAsync(e.Message, $"{Consts.FAILED_RETRIEVING_USER_STR} {email}");
                return new BadRequestObjectResult(new { Message = Consts.FAILED_RETRIEVING_USER_STR, Error = Consts.FAILED_RETRIEVING_USER_DESC_STR });
            }
            catch (OperationCanceledException e)
            {
                //await _loggerService.LogErrorAsync(e.Message, Consts.DATABASE_CONNECTION_TIMEOUT_STR);
                return new ObjectResult(new { Message = Consts.REQUEST_TIME_OUT_STR, Error = Consts.DATABASE_CONNECTION_TIMEOUT_STR })
                {
                    StatusCode = StatusCodes.Status408RequestTimeout
                };
            }          
        }

        public string HashPassword(string password)
        {
            return BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Verify(password, hashedPassword);
        }
    }
}
