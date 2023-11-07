using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Middlewares;

public class RequestIdMiddleware
{
    private readonly ILogger<RequestIdMiddleware> _logger;
    private readonly RequestDelegate _next;

    public RequestIdMiddleware(RequestDelegate next, ILogger<RequestIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        // Log the request details
        httpContext.TraceIdentifier = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;

        await _next(httpContext);

        httpContext.Response.Headers.Add("X-Request-Id", httpContext.TraceIdentifier);
    }
}