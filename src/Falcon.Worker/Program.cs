using Falcon.Infrastructure;
using Falcon.Worker.Consumers;
using MassTransit;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add Infrastructure (DbContext, IJudgeService, etc.)
        services.AddInfrastructure(context.Configuration);

        // Configure MassTransit with Consumer
        services.AddMassTransit(x =>
        {
            x.AddConsumer<SubmitExerciseConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });
    })
    .Build();

await host.RunAsync();
