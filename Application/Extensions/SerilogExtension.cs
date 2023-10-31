using Serilog;

namespace Application.Extensions;

public static class SerilogExtension
{
    public static void AddSerilog(this IServiceCollection services)
    {
        const string outputTemplate =
            "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.WithCorrelationId()
            .Enrich.WithCorrelationIdHeader()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .CreateLogger();

        services.AddLogging(builder => builder.AddSerilog());
    }
}