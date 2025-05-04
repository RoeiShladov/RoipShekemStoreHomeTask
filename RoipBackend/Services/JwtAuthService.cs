using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RoipBackend.Utilities;

namespace RoipBackend.Services
{
    public class JwtAuthService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly LoggerService _loggerService; // Updated to use ILoggerService interface

        public JwtAuthService(string secretKey, string issuer, string audience, LoggerService loggerService)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
            _loggerService = loggerService; // Updated to use ILoggerService
        }

        public async Task<string> GenerateJWT(string userId, string userName, string email, string role, int expirationMinutes)
        {
            try
            {
                // Set the expiration time based on the provided expirationMinutes parameter
                var expirationTime = DateTime.UtcNow.AddMinutes(expirationMinutes);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iss, _issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, _audience),
                    new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expirationTime).ToUnixTimeSeconds().ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var creds = new SigningCredentials(key, C.JWT_SIGN_ALGORITHM_STR);

                var jwt = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: expirationTime,
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(jwt);
            }
            catch (ArgumentNullException ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, $"{C.ARGUMENT_NULL_STR}: {C.ARGUMENT_NULL_DESC_STR}");
                return string.Empty;
            }
            catch (SecurityTokenException ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, C.SECURITY_TOKEN_EXCEPTION_STR);
                return string.Empty;
            }
            catch (Exception ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, C.INTERNAL_SERVER_ERROR_STR);
                return string.Empty;
            }
        }

        public async Task<bool> IsMissingOrInvalidContent(string jwt)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            try
            {
                jwtHandler.ValidateToken(jwt, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                    ValidAlgorithms = [C.JWT_SIGN_ALGORITHM_STR],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedJWT);
            }
            catch (SecurityTokenExpiredException ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, C.JWT_EXPIRED_STR);
                return true;
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, C.INVALID_SIGNATURE_STR);
                return true;
            }
            catch (Exception ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, C.INTERNAL_SERVER_ERROR_STR);
                return true;
            }
            return false;
        }

        public async Task<ClaimsPrincipal> GetJwtClaims(string jwt)
        {
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero,
                    ValidAlgorithms = [C.JWT_SIGN_ALGORITHM_STR]
                };

                return jwtHandler.ValidateToken(jwt, validationParameters, out _);
            }
            catch (SecurityTokenExpiredException ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, C.JWT_EXPIRED_STR);
                return null;
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, C.INVALID_SIGNATURE_STR);
                return null;
            }
            catch (Exception ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, C.INTERNAL_SERVER_ERROR_STR);
                return null;
            }
        }

        public async Task<bool> IsClaimsOk(string jwt, string role)
        {
            try
            {
                bool isOk = true;
                var claimsPrincipal = await GetJwtClaims(jwt); // Await the Task to get the ClaimsPrincipal object
                if (claimsPrincipal == null)
                    return false;

                var expirationClaim = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Exp);
                if (expirationClaim != null)
                {
                    var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim.Value)).UtcDateTime;
                    isOk = expirationDate > DateTime.UtcNow; // Corrected comparison to check if expirationDate is in the future
                }

                var roleClaim = claimsPrincipal.FindFirst(ClaimTypes.Role);
                if (roleClaim == null || roleClaim.Value != role)
                    isOk = false;

                var issuerClaim = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Iss);
                isOk = issuerClaim != null && issuerClaim.Value == _issuer;

                return isOk;
            }
            catch (FormatException ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, C.FORMAT_EXCEPTION_STR);
                return false;
            }
            catch (Exception ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, C.INTERNAL_SERVER_ERROR_STR);
                return false;
            }
        }

        public async Task<ServiceResult<string>> JwtValidationCheck(string jwt, string role)
        {
            try
            {
                // Await the tasks to get their results before continuing code.
                bool isMissingOrInvalid = await IsMissingOrInvalidContent(jwt);
                bool isClaimsInvalid = await IsClaimsOk(jwt, role);

                if (isMissingOrInvalid || isClaimsInvalid)
                {
                    return new ServiceResult<string>
                    {
                        Success = false,
                        Message = C.JWT_INVALID_STR,
                        Error = C.JWT_INVALID_DESC_STR,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        RoipShekemStoreJWT = string.Empty
                    };
                }

                return new ServiceResult<string>
                {
                    Success = true,
                    Message = C.JWT_VALID_STR,
                    StatusCode = StatusCodes.Status200OK,
                    RoipShekemStoreJWT = jwt
                };
            }
            catch (ArgumentNullException ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, $"{C.ARGUMENT_NULL_STR}: {C.ARGUMENT_NULL_DESC_STR}");
                return new ServiceResult<string>
                {
                    Success = false,
                    Message = C.JWT_INVALID_STR,
                    Error = C.JWT_INVALID_DESC_STR,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    RoipShekemStoreJWT = string.Empty
                };
            }
            catch (Exception ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, C.INTERNAL_SERVER_ERROR_STR);
                return new ServiceResult<string>
                {
                    Success = false,
                    Message = C.JWT_GENERATION_FAILED_DESC_STR,
                    Error = C.INTERNAL_SERVER_ERROR_STR,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    RoipShekemStoreJWT = string.Empty
                };
            }
        }

        public async Task<ServiceResult<string>> JwtContentCheck(string jwt, string role)
        {
            try
            {
                if(string.IsNullOrEmpty(jwt))
                    return new ServiceResult<string>
                    {
                        Success = false,
                        Message = C.JWT_INVALID_STR,
                        Error = C.JWT_INVALID_DESC_STR,
                        StatusCode = StatusCodes.Status401Unauthorized
                    };

                ServiceResult<string> validationResult = await JwtValidationCheck(jwt, role);
                if (!validationResult.Success)
                    await _loggerService.LogWarningAsync(
                        $"{validationResult.Message}. {validationResult.Error}. {validationResult.StatusCode}");

                return validationResult;
            }
            catch (ArgumentNullException ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, $"{C.ARGUMENT_NULL_STR}: {C.ARGUMENT_NULL_DESC_STR}");
                return new ServiceResult<string>
                {
                    Success = false,
                    Message = C.JWT_INVALID_STR,
                    Error = C.JWT_INVALID_DESC_STR,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    RoipShekemStoreJWT = string.Empty
                };
            }
            catch (Exception ex)
            {
                await _loggerService.LogErrorAsync(ex.Message, C.INTERNAL_SERVER_ERROR_STR);
                return new ServiceResult<string>
                {
                    Success = false,
                    Message = C.JWT_GENERATION_FAILED_DESC_STR,
                    Error = C.INTERNAL_SERVER_ERROR_STR,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    RoipShekemStoreJWT = string.Empty
                };
            }
        }
    }
}