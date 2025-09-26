using System.Net;
using System.Text.Json;

namespace PaymentsModuleAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = GetExceptionDetails(exception);

        var response = new
        {
            Title = GetErrorTitle(statusCode),
            Status = (int)statusCode,
            Detail = message,
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static (HttpStatusCode statusCode, string message) GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => (HttpStatusCode.BadRequest, exception.Message),
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
            NotImplementedException => (HttpStatusCode.NotImplemented, exception.Message),
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An internal server error occurred")
        };
    }

    private static string GetErrorTitle(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "Bad Request",
            HttpStatusCode.NotImplemented => "Not Implemented",
            HttpStatusCode.NotFound => "Not Found",
            HttpStatusCode.InternalServerError => "Internal Server Error",
            _ => "Error"
        };
    }
}