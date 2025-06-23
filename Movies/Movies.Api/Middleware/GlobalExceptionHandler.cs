using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Movies.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            var validationErrors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());
            
            var validationProblemDetails = new ValidationProblemDetails
            {
                Title = "Validation Failure",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1", // link documentation for error if you have one.
                Errors = validationErrors,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["traceId"] = httpContext.TraceIdentifier,
                    ["method"] = httpContext.Request.Method
                }
            };

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);
            return true;
        }

        
        // for all other server errors
        // var error = exception.Message; for logging the errors.

        var problemDetails = new ProblemDetails
        {
            Title = "An unexpected error occurred.",
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1", 
            Detail = "An internal server error occurred. Please contact support with the trace ID.",
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,
                ["method"] = httpContext.Request.Method
            }
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}


// problemDetails.Extensions["correlationId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier; // for microservices.