using Application.Logging;
using Serilog;
using Serilog.Exceptions;

namespace Application.Extensions;

public static class SerilogExtension
{
    public static void AddSerilog(this IServiceCollection services, string outputTemplate)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .Enrich.WithExceptionDetails()
            .Enrich.FromLogContext()
            .Enrich.With<CustomEnricher>()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .CreateLogger();

        services.AddLogging(builder => builder.AddSerilog());
    }
}