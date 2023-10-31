using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Middlewares;

public class LoggingMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        // Log the request details
        await LogRequest(httpContext.Request);

        // Capture the response body
        var originalBodyStream = httpContext.Response.Body;
        using (var responseBody = new MemoryStream())
        {
            httpContext.Response.Body = responseBody;

            await _next(httpContext);

            // Log the response details
            await LogResponse(httpContext.Response);

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task LogRequest(HttpRequest httpRequest)
    {
        httpRequest.EnableBuffering();

        var body = await ReadRequestBody(httpRequest);
        httpRequest.Body.Seek(0, SeekOrigin.Begin);


        var requestLog = new
        {
            httpRequest.Method,
            Path = httpRequest.Path.Value,
            QueryString = httpRequest.QueryString.Value,
            Headers = new
            {
                httpRequest.Headers.Accept,
                httpRequest.Headers.Host,
                httpRequest.Headers.Authorization,
                httpRequest.Headers.UserAgent,
                httpRequest.Headers.Referer
            },
            Body = body
        };
        _logger.LogInformation("Request: {@Request}", JsonSerializer.Serialize(requestLog));
    }

    private async Task LogResponse(HttpResponse httpResponse)
    {
        var body = await ReadResponseBody(httpResponse);

        var responseLog = new
        {
            Body = body,
            httpResponse.StatusCode
        };

        _logger.LogInformation("Response: {@Response}", JsonSerializer.Serialize(responseLog));
    }

    private static async Task<string> ReadRequestBody(HttpRequest httpRequest)
    {
        httpRequest.EnableBuffering();

        using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8, true, 1024, true);
        return await reader.ReadToEndAsync();
    }

    private static async Task<string> ReadResponseBody(HttpResponse httpResponse)
    {
        httpResponse.Body.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(httpResponse.Body, Encoding.UTF8, true, 1024, true);
        return await reader.ReadToEndAsync();
    }
}