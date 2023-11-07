using System.Web;
using Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<RabbitMqService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IRabbitMqService _rabbitMqService;

    public NotificationService(ILogger<RabbitMqService> logger, IConfiguration configuration,
        IRabbitMqService rabbitMqService)
    {
        _logger = logger;
        _configuration = configuration;
        _rabbitMqService = rabbitMqService;
    }

    public Task SendEmailConfirmation(IdentityUser user, string emailConfirmationToken)
    {
        using var connection = _rabbitMqService.CreateConnection();
        using var channel = _rabbitMqService.CreateChannel(connection);

        channel.QueueDeclare(queue: "notification.email-confirmation", durable: true, exclusive: false, autoDelete: false,
            arguments: null);

        var confirmationLink = new UriBuilder(new Uri("http://localhost/User/Confirmation", UriKind.Absolute))
            { Port = -1 };

        var query = HttpUtility.ParseQueryString(confirmationLink.Query);
        query["userId"] = user.Id;
        query["emailConfirmationToken"] = emailConfirmationToken;

        confirmationLink.Query = query.ToString();

        var message = $"Olá, seja bem vindo! Seu link de confirmação: {confirmationLink}";

        _rabbitMqService.PublishMessage(channel, message, "notification.email-confirmation",null);

        _logger.LogInformation("Message sent! {Message}", message);
        return Task.CompletedTask;
    }

    public Task SendPasswordReset(IdentityUser user, string passwordConfirmationToken)
    {
        using var connection = _rabbitMqService.CreateConnection();
        using var channel = _rabbitMqService.CreateChannel(connection);

        channel.QueueDeclare(queue: "notification.password-reset", durable: true, exclusive: false, autoDelete: false,
            arguments: null);

        var confirmationLink = new UriBuilder(new Uri("http://localhost/User/Password-Reset", UriKind.Absolute))
            { Port = -1 };

        var query = HttpUtility.ParseQueryString(confirmationLink.Query);
        query["userId"] = user.Id;
        query["passwordResetToken"] = passwordConfirmationToken;

        confirmationLink.Query = query.ToString();

        var message = $"Olá, seja bem vindo! Seu link para resetar senha: {confirmationLink}";

        _rabbitMqService.PublishMessage(channel, message, "notification.password-reset",null);

        _logger.LogInformation("Message sent! {Message}", message);
        return Task.CompletedTask;
    }
}