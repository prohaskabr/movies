using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Movies.Api.Auth;

public class ApiKeyAuthFilter(IConfiguration configuration) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var headerApiKey))
        {

            context.Result = new UnauthorizedObjectResult("Api key missing");
            return;
        }

        var apiKey = configuration["ApiKey"];

        if (apiKey != headerApiKey)
        {
            context.Result = new UnauthorizedObjectResult("Invalid api key");
            return;
        }
    }
}
