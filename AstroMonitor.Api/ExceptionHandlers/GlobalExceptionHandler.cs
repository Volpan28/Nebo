using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AstroMonitor.Api.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        if (exception is FluentValidation.ValidationException validationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "about:blank",
                Title = "Validation failed",
                Detail = "One or more validation errors occurred."
            };
            
            problemDetails.Extensions["errors"] = errors;
            
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
        
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
        
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var serverErrorDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "about:blank",
            Title = "Internal Server Error",
            Detail = "An internal server error has occurred. Please try again later."
        };
        
        await httpContext.Response.WriteAsJsonAsync(serverErrorDetails, cancellationToken);
        return true;
    }
}