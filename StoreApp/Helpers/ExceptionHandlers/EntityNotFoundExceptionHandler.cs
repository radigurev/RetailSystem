using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Shared.Exceptions;

namespace StoreApp.Helpers.ExceptionHandlers;

public class EntityNotFoundExceptionHandler(ILogger<EntityNotFoundExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<EntityNotFoundExceptionHandler> _logger = logger;
    
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        if (exception is not EntityNotFoundException)
            return ValueTask.FromResult(false);
        
        _logger.LogInformation(exception, exception.Message);
        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        httpContext.Response.WriteAsync(exception.Message, cancellationToken: cancellationToken);

        return ValueTask.FromResult(true);
    }
}