using Falcon.Infrastructure;
using Falcon.Worker.Consumers;
using MassTransit;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (context, services) =>
        {
            // Add Infrastructure (DbContext, IJudgeService, etc.)
            services.AddInfrastructure(context.Configuration);

            // Configure MassTransit with Consumer
            services.AddMassTransit(x =>
            {
                string host = context.Configuration.GetValue<string>("RabbitMQ:Host", "localhost");
                string username = context.Configuration.GetValue<string>(
                    "RabbitMQ:Username",
                    "guest"
                );
                string password = context.Configuration.GetValue<string>(
                    "RabbitMQ:Password",
                    "guest"
                );

                x.AddConsumer<SubmitExerciseConsumer>();

                x.UsingRabbitMq(
                    (ctx, cfg) =>
                    {
                        cfg.Host(
                            host,
                            "/",
                            h =>
                            {
                                h.Username(username);
                                h.Password(password);
                            }
                        );

                        cfg.ConfigureEndpoints(ctx);
                    }
                );
            });
        }
    )
    .Build();

await host.RunAsync();
