var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddHostedService<Worker.Worker>(); })
    .Build();

host.Run();