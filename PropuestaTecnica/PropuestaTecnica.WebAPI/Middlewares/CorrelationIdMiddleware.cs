namespace PropuestaTecnica.WebAPI.Middlewares;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Leer el ID si viene del cliente, sino generarlo
        if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers.Append(HeaderName, correlationId);
        }

        // También se agrega a la respuesta
        context.Response.Headers.Append(HeaderName, correlationId);

        // Guardar el ID en Items, para que Serilog lo capture
        context.Items[HeaderName] = correlationId;

        await _next(context);
    }
}
