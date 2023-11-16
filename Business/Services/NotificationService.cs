using System.Web;
using Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class NotificationService : INotificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqService> _logger;
    private readonly IRabbitMqService _rabbitMqService;

    public NotificationService(ILogger<RabbitMqService> logger, IConfiguration configuration,
        IRabbitMqService rabbitMqService)
    {
        _logger = logger;
        _configuration = configuration;
        _rabbitMqService = rabbitMqService;
    }

    public Task PublishEmailConfirmation(IdentityUser user, string emailConfirmationToken)
    {
        using var connection = _rabbitMqService.CreateConnection();
        using var channel = _rabbitMqService.CreateChannel(connection);

        channel.QueueDeclare("notification.email-confirmation", true, false, false,
            null);

        var confirmationUrl = new UriBuilder(new Uri("http://localhost:5112/User/verify-email", UriKind.Absolute))
            { Port = 5112 };

        var query = HttpUtility.ParseQueryString(confirmationUrl.Query);
        query["userId"] = user.Id;
        query["emailConfirmationToken"] = emailConfirmationToken;

        confirmationUrl.Query = query.ToString();

        var message = confirmationUrl.ToString();

        _rabbitMqService.PublishMessage(channel, message, "notification.email-confirmation", null);

        _logger.LogInformation("Message sent! {Message}", message);
        return Task.CompletedTask;
    }

    public Task PublishResetPassword(IdentityUser user, string resetPasswordToken)
    {
        using var connection = _rabbitMqService.CreateConnection();
        using var channel = _rabbitMqService.CreateChannel(connection);

        channel.QueueDeclare("notification.password-reset", true, false, false,
            null);

        var resetUrl = new UriBuilder(new Uri("http://localhost:5112/User/reset-password", UriKind.Absolute))
            { Port = 5112 };

        var query = HttpUtility.ParseQueryString(resetUrl.Query);
        query["userId"] = user.Id;
        query["resetPasswordToken"] = resetPasswordToken;

        resetUrl.Query = query.ToString();

        var message = resetUrl.ToString();

        _rabbitMqService.PublishMessage(channel, message, "notification.password-reset", null);

        _logger.LogInformation("Message sent! {Message}", message);
        return Task.CompletedTask;
    }
}