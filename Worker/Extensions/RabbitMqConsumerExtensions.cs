using Application.Extensions;
using Business.Interfaces;
using Business.Services;

namespace Worker.Extensions;

public static class RabbitMqConsumerExtensions
{
    public static IHostBuilder ConfigureRabbitMqConsumers(this IHostBuilder hostBuilder,
        Action<IServiceCollection> action)
    {
        return hostBuilder.ConfigureServices((context, collection) =>
        {
            action?.Invoke(collection);
        });
    }
}