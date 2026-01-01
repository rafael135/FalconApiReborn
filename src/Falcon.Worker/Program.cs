using Falcon.Infrastructure;
using Falcon.Worker.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
