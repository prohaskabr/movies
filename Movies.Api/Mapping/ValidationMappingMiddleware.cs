using FluentValidation;
using Movies.Contracts.Responses;


namespace Movies.Api.Mapping;

public class ValidationMappingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {        
        try
        {
            await next(context);
        }
        catch (ValidationException e)
        {
            context.Response.StatusCode = 400;
            var failureResponse = new ValidationFailureResponse
            {
                Errors = e.Errors.Select(x => new ValidationResponse
                {
                    PropertyName = x.PropertyName,
                    Message = x.ErrorMessage,
                })
            };
            
            await context.Response.WriteAsJsonAsync(failureResponse);
        }
    }
}
