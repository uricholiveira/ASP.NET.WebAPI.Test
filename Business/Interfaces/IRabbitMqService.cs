using RabbitMQ.Client;

namespace Business.Interfaces;

public interface IRabbitMqService
{
    public IConnection CreateConnection();
    public IModel CreateChannel(IConnection connection);
    public IBasicProperties CreateBasicProperties(IModel channel);
    public void PublishMessage(IModel channel, string message, string queue, IBasicProperties? basicProperties);
    public string ConsumeMessage(IModel channel, string queue, Action<string, ulong> messageHandler);
}