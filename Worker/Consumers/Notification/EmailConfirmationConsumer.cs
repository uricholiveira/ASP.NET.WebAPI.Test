using System.Diagnostics;
using Business.Interfaces;

namespace Worker.Consumers.Notification;

public class EmailConfirmationConsumer : BackgroundService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<PasswordResetConsumer> _logger;
    private readonly IRabbitMqService _rabbitMqService;

    public EmailConfirmationConsumer(ILogger<PasswordResetConsumer> logger, IRabbitMqService rabbitMqService,
        IEmailService emailService)
    {
        _logger = logger;
        _rabbitMqService = rabbitMqService;
        _emailService = emailService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO: Fazer isso automagicamente
        if (Activity.Current is null)
        {
        }

        var activity = new Activity(nameof(PasswordResetConsumer));

        activity.Start();

        using var connection = _rabbitMqService.CreateConnection();
        using var channel = _rabbitMqService.CreateChannel(connection);

        channel.QueueDeclare("notification.email-confirmation", true, false,
            false,
            null);

        var message = _rabbitMqService.ConsumeMessage(channel, "notification.email-confirmation",
            message => { _emailService.SendEmailConfirmation(message, "oliveira.urich@gmail.com"); });

        _logger.LogInformation("Message received: {Message}", message);


        activity.Stop();
        while (!stoppingToken.IsCancellationRequested) await Task.Delay(1000, stoppingToken);
    }
}