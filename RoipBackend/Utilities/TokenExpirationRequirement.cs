using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

public class TokenExpirationRequirement : IAuthorizationRequirement { }

public class TokenExpirationHandler : AuthorizationHandler<TokenExpirationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenExpirationRequirement requirement)
    {
        var tokenClaim = context.User.FindFirst("exp");
        if (tokenClaim != null)
        {
            var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tokenClaim.Value));
            if (expirationDate > DateTimeOffset.UtcNow)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
