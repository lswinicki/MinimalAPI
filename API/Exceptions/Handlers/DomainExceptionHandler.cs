using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Exceptions.Handlers;

public class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainException)
            return false;
        
        httpContext.Response.StatusCode = 400;
        httpContext.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails()
        {
            Status = 400,
            Title = "Bad Request",
            Detail = exception.Message
        };
        
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}