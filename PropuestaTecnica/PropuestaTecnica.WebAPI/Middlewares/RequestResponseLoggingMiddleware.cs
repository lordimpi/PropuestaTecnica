using System.Text;

namespace PropuestaTecnica.WebAPI.Middlewares;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Items["X-Correlation-Id"]?.ToString() ?? "N/A";

        // LOG REQUEST
        context.Request.EnableBuffering();

        string requestBody = "";
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        _logger.LogInformation(
            "HTTP Request → {method} {url} | CorrelationId: {correlationId} | Body: {body}",
            context.Request.Method,
            context.Request.Path,
            correlationId,
            requestBody
        );

        // LOG RESPONSE
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        string responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        _logger.LogInformation(
            "HTTP Response ← {statusCode} | CorrelationId: {correlationId} | Body: {body}",
            context.Response.StatusCode,
            correlationId,
            responseText
        );

        await responseBody.CopyToAsync(originalBodyStream);
    }
}