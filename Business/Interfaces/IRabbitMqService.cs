using RabbitMQ.Client;

namespace Business.Interfaces;

public interface IRabbitMqService
{
    public IConnection CreateConnection();
    public IModel CreateChannel(IConnection connection);
    public IBasicProperties CreateBasicProperties(IModel channel);
    public void PublishMessage(IModel channel, string message, IBasicProperties? basicProperties);
    public void ConsumeMessage();
}