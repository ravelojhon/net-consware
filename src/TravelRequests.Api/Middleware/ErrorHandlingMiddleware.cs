using System.Net;
using System.Text.Json;
using Serilog;
using TravelRequests.Domain.Exceptions;

namespace TravelRequests.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case ValidationException valEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new ErrorResponse
                {
                    Error = "Validation Error",
                    Message = valEx.Message,
                    Details = GetExceptionDetails(valEx)
                };
                _logger.LogWarning(valEx, "Validation error: {Message}", valEx.Message);
                break;

            case NotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse = new ErrorResponse
                {
                    Error = "Not Found",
                    Message = notFoundEx.Message,
                    Details = GetExceptionDetails(notFoundEx)
                };
                _logger.LogWarning(notFoundEx, "Resource not found: {Message}", notFoundEx.Message);
                break;

            case UnauthorizedException unauthorizedEx:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse = new ErrorResponse
                {
                    Error = "Unauthorized",
                    Message = unauthorizedEx.Message,
                    Details = GetExceptionDetails(unauthorizedEx)
                };
                _logger.LogWarning(unauthorizedEx, "Unauthorized access: {Message}", unauthorizedEx.Message);
                break;

            case BusinessRuleException businessEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new ErrorResponse
                {
                    Error = "Business Rule Violation",
                    Message = businessEx.Message,
                    Details = GetExceptionDetails(businessEx)
                };
                _logger.LogWarning(businessEx, "Business rule violation: {Message}", businessEx.Message);
                break;

            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new ErrorResponse
                {
                    Error = "Bad Request",
                    Message = argEx.Message,
                    Details = GetExceptionDetails(argEx)
                };
                _logger.LogWarning(argEx, "Bad Request: {Message}", argEx.Message);
                break;


            case UnauthorizedAccessException authEx:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse = new ErrorResponse
                {
                    Error = "Unauthorized",
                    Message = authEx.Message,
                    Details = GetExceptionDetails(authEx)
                };
                _logger.LogWarning(authEx, "Unauthorized access: {Message}", authEx.Message);
                break;

            case InvalidOperationException opEx:
                // Determinar si es 400 o 500 basado en el contexto
                if (IsClientError(opEx))
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new ErrorResponse
                    {
                        Error = "Bad Request",
                        Message = opEx.Message,
                        Details = GetExceptionDetails(opEx)
                    };
                    _logger.LogWarning(opEx, "Invalid operation: {Message}", opEx.Message);
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new ErrorResponse
                    {
                        Error = "Internal Server Error",
                        Message = "An unexpected error occurred",
                        Details = GetExceptionDetails(opEx)
                    };
                    _logger.LogError(opEx, "Internal server error: {Message}", opEx.Message);
                }
                break;


            case TimeoutException timeoutEx:
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse = new ErrorResponse
                {
                    Error = "Request Timeout",
                    Message = "The request timed out",
                    Details = GetExceptionDetails(timeoutEx)
                };
                _logger.LogWarning(timeoutEx, "Request timeout: {Message}", timeoutEx.Message);
                break;

            case NotImplementedException notImplEx:
                response.StatusCode = (int)HttpStatusCode.NotImplemented;
                errorResponse = new ErrorResponse
                {
                    Error = "Not Implemented",
                    Message = "This feature is not yet implemented",
                    Details = GetExceptionDetails(notImplEx)
                };
                _logger.LogWarning(notImplEx, "Not implemented: {Message}", notImplEx.Message);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = new ErrorResponse
                {
                    Error = "Internal Server Error",
                    Message = "An unexpected error occurred",
                    Details = GetExceptionDetails(exception)
                };
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }

        // Log del error con contexto adicional
        Log.Error("Error occurred: {ErrorType} - {Message} - Path: {Path} - Method: {Method} - StatusCode: {StatusCode}",
            exception.GetType().Name,
            exception.Message,
            context.Request.Path,
            context.Request.Method,
            response.StatusCode);

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await response.WriteAsync(jsonResponse);
    }

    private static bool IsClientError(InvalidOperationException ex)
    {
        // Determinar si es un error del cliente basado en el mensaje o tipo
        var clientErrorMessages = new[]
        {
            "cannot be updated",
            "cannot be deleted",
            "cannot be created",
            "already exists",
            "not found",
            "invalid",
            "required",
            "expired",
            "used"
        };

        return clientErrorMessages.Any(msg => 
            ex.Message.Contains(msg, StringComparison.OrdinalIgnoreCase));
    }

    private static object? GetExceptionDetails(Exception ex)
    {
        // En desarrollo, incluir más detalles
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            return new
            {
                ExceptionType = ex.GetType().Name,
                StackTrace = ex.StackTrace,
                InnerException = ex.InnerException?.Message
            };
        }

        // En producción, solo información básica
        return new
        {
            ExceptionType = ex.GetType().Name,
            Timestamp = DateTime.UtcNow
        };
    }
}

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; }
}
