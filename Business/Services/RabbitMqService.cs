using System.Diagnostics;
using System.Text;
using Business.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Business.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly ILogger<RabbitMqService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IConnectionFactory _connectionFactory;

    public RabbitMqService(ILogger<RabbitMqService> logger, IConfiguration configuration,
        IConnectionFactory connectionFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _connectionFactory = connectionFactory;
    }

    public IConnection CreateConnection()
    {
        try
        {
            return _connectionFactory.CreateConnection();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar conexão RabbitMQ.");
            throw;
        }
    }

    public IModel CreateChannel(IConnection connection)
    {
        try
        {
            return connection.CreateModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar canal RabbitMQ.");
            throw;
        }
    }

    public IBasicProperties CreateBasicProperties(IModel channel)
    {
        try
        {
            return channel.CreateBasicProperties();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar propriedades básicas do RabbitMQ.");
            throw;
        }
    }

    public void PublishMessage(IModel channel, string message, string queue, IBasicProperties? basicProperties = null)
    {
        try
        {
            basicProperties ??= channel.CreateBasicProperties();

            basicProperties.CorrelationId = Activity.Current!.TraceId.ToString();
            channel.BasicPublish(exchange: "web-services", routingKey: queue, basicProperties: basicProperties,
                body: Encoding.UTF8.GetBytes(message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem RabbitMQ.");
            throw;
        }
    }

    public string ConsumeMessage(IModel channel, string queue, Action<string> messageHandler)
    {
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var parentId = ea.BasicProperties.CorrelationId;

            Activity.Current?.SetParentId(parentId);

            messageHandler(message);
        };

        return channel.BasicConsume(queue, true, consumer);
    }
}