using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Movies.Api.Auth;

public class AdminAuthRequirement : IAuthorizationHandler, IAuthorizationRequirement
{
    private readonly string apiKey;

    public AdminAuthRequirement(string apiKey)
    {
        this.apiKey = apiKey;
    }

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.User.HasClaim(AuthConstants.AdminUserClaimName, "true"))
        {
            context.Succeed(this);
            return Task.CompletedTask;
        }

        var httpContext = context.Resource as HttpContext;
        if (httpContext is null)
            return Task.CompletedTask;


        if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var headerApiKey))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (apiKey != headerApiKey)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var identity = (ClaimsIdentity)httpContext.User.Identity!;
        identity.AddClaim(new Claim("userid", "53ca653a-54ce-47c7-af6b-9b17eb4666f4"));
        context.Succeed(this);

        return Task.CompletedTask;
    }
}
