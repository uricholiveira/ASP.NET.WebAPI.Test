using System.Diagnostics;
using System.Web;
using Business.Interfaces;

namespace Worker.Consumers.Notification;

public class ChangePasswordConsumer : BackgroundService
{
    private readonly ILogger<ChangePasswordConsumer> _logger;
    private readonly IRabbitMqService _rabbitMqService;

    public ChangePasswordConsumer(ILogger<ChangePasswordConsumer> logger, IRabbitMqService rabbitMqService)
    {
        _logger = logger;
        _rabbitMqService = rabbitMqService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO: Fazer isso automagicamente
        if (Activity.Current is null)
        {
        }
        var activity = new Activity(nameof(ChangePasswordConsumer));
        
        activity.Start();
        
        using var connection = _rabbitMqService.CreateConnection();
        using var channel = _rabbitMqService.CreateChannel(connection);

        channel.QueueDeclare(queue: "notification.password-reset", durable: true, exclusive: false,
            autoDelete: false,
            arguments: null);

        var message = _rabbitMqService.ConsumeMessage(channel, "notification.password-reset",
            message => { _logger.LogInformation($"Inside handler: {message}"); });

        _logger.LogInformation("Message received: {Message}", message);

        
        activity.Stop();
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}