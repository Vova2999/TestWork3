using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;

namespace Server.Exceptions;

public class GlobalExceptionsHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionsHandler> _logger;

    public GlobalExceptionsHandler(
        ILogger<GlobalExceptionsHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (code, message) = (HttpStatusCode.InternalServerError, "Неизвестная ошибка");

        _logger.LogDebug(exception, $"Handled exception with code {code}");

        httpContext.Response.StatusCode = (int) code;
        httpContext.Response.ContentType = MediaTypeNames.Text.Plain;
        await httpContext.Response.WriteAsync(message, cancellationToken);

        return true;
    }
}