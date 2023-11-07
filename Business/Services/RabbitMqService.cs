using System.Text;
using Business.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

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

    public void PublishMessage(IModel channel, string message, IBasicProperties? basicProperties = null)
    {
        try
        {
            // Substitua "exchange" e "routingKey" com os valores apropriados para o seu caso
            channel.BasicPublish(exchange: "web-services", routingKey: "notification", basicProperties: basicProperties,
                body: Encoding.UTF8.GetBytes(message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem RabbitMQ.");
            throw;
        }
    }

    public void ConsumeMessage()
    {
        throw new NotImplementedException();
    }
}