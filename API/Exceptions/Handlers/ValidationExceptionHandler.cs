using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Exceptions.Handlers;

public class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
            return false;
        
        httpContext.Response.StatusCode = 400;
        httpContext.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails()
        {
            Status = 400,
            Title = "Bad Request",
            Detail = "One or more fields failed validation.",
        };
        
        problemDetails.Extensions.Add("errors", validationException.Errors.Select(x => new {x.PropertyName, x.ErrorMessage}));
        
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}