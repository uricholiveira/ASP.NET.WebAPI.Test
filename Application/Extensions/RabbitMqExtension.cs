using Business.Interfaces;
using Business.Services;
using Data.Models;
using RabbitMQ.Client;

namespace Application.Extensions;

public static class RabbitMqExtension
{
    public static void AddRabbitMq(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var rabbitMqConfiguration = configuration.GetSection("RabbitMQ").Get<RabbitMqConfiguration>();

        var connectionFactory = new ConnectionFactory
        {
            HostName = rabbitMqConfiguration?.Host,
            Port = rabbitMqConfiguration!.Port,
            UserName = rabbitMqConfiguration?.Username,
            Password = rabbitMqConfiguration?.Password
        };

        serviceCollection.AddSingleton<IConnectionFactory>(connectionFactory);
        serviceCollection.AddSingleton<IRabbitMqService, RabbitMqService>();
    }
}