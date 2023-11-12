using Application.Extensions;
using Business.Interfaces;
using Business.Services;
using Serilog;
using Worker.Consumers.Notification;
using Worker.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        const string outputTemplate =
            "[{Timestamp:HH:mm:ss} {Level:u3}] [{ParentId}] {Message:lj}{NewLine}{Exception}";

        services.AddSerilog(outputTemplate);
        services.AddHostedService<Worker.Worker>();
        services.AddRabbitMq(configuration);

        services.AddSingleton<EmailSettings>(provider => new EmailSettings
        {
            SmtpServer = configuration["Email:SmtpSettings:SmtpServer"]!,
            SmtpPort = int.Parse(configuration["Email:SmtpSettings:SmtpPort"]!),
            SmtpUsername = configuration["Email:SmtpSettings:SmtpUsername"]!,
            SmtpPassword = configuration["Email:SmtpSettings:SmtpPassword"]!,
            SmtpFrom = configuration["Email:SmtpSettings:SmtpFrom"]!
        });
        services.AddSingleton<IEmailService, EmailService>();
    })
    .ConfigureRabbitMqConsumers(collection =>
    {
        collection.AddHostedService<PasswordResetConsumer>();
        collection.AddHostedService<EmailConfirmationConsumer>();
    })
    .UseSerilog()
    .Build();

host.Run();