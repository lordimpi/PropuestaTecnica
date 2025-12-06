using PropuestaTecnica.WebAPI.Models;
using System.Net;

namespace PropuestaTecnica.WebAPI.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception caught by global middleware.");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        string clientMessage =
            "Ha ocurrido un error inesperado procesando la solicitud. Por favor inténtalo nuevamente.";

        string? detail = _env.IsDevelopment() ? ex.Message : null;

        var response = new ApiResponse<string>
        {
            Success = false,
            Message = clientMessage,
            Data = detail
        };

        var json = System.Text.Json.JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}