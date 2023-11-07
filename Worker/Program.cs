using Application.Extensions;
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
    })
    .ConfigureRabbitMqConsumers(collection =>
    {
        collection.AddHostedService<ChangePasswordConsumer>();
    })
    .UseSerilog()
    .Build();

host.Run();