using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;

namespace Shared.ExceptionHandlers;

public class DuplicateProductExceptionHandler(ILogger<DuplicateProductExceptionHandler> _logger) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        if (exception is not DuplicateProductException)
            return ValueTask.FromResult(false);
        
        _logger.LogInformation(exception, exception.Message);
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.WriteAsync(exception.Message, cancellationToken: cancellationToken);

        return ValueTask.FromResult(true);
    }
}