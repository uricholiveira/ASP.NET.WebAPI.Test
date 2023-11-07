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

        channel.QueueDeclare(queue: "notification", durable: true, exclusive: false, autoDelete: false,
            arguments: null);

        var confirmationLink = new UriBuilder(new Uri("http://localhost/User/Confirmation", UriKind.Absolute))
            { Port = -1 };

        var query = HttpUtility.ParseQueryString(confirmationLink.Query);
        query["userId"] = user.Id;
        query["emailConfirmationToken"] = emailConfirmationToken;

        confirmationLink.Query = query.ToString();

        var message = $"Olá, seja bem vindo! Seu link de confirmação: {confirmationLink}";

        _rabbitMqService.PublishMessage(channel, message, null);

        _logger.LogInformation("Message sent! {Message}", message);
        return Task.CompletedTask;
    }
}