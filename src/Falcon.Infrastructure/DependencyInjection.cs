using Falcon.Core.Domain.Users;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Auth;
using Falcon.Infrastructure.Database;
using Falcon.Infrastructure.Judge;
using Falcon.Infrastructure.Storage;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Falcon.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<FalconDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    );
                }
            )
        );

        services
            .AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<FalconDbContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders();

        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            bus.UsingRabbitMq(
                (context, cfg) =>
                {
                    cfg.Host(
                        "localhost",
                        "/",
                        h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        }
                    );

                    cfg.ConfigureEndpoints(context);
                }
            );
        });

        // Configure HttpClient for Judge API
        var judgeApiUrl = configuration["JudgeApi:Url"];
        if (!string.IsNullOrEmpty(judgeApiUrl))
        {
            services.AddHttpClient("JudgeAPI", client =>
            {
                client.BaseAddress = new Uri(judgeApiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            });
        }

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IJudgeService, JudgeService>();
        services.AddSingleton<IFileStorageService, LocalFileStorageService>();

        services.AddHttpContextAccessor();

        return services;
    }

    /// <summary>
    /// Adds MassTransit configuration for API with consumers.
    /// </summary>
    public static IServiceCollection AddApiMassTransit(
        this IServiceCollection services,
        Action<IRegistrationConfigurator> configureConsumers)
    {
        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            configureConsumers(bus);

            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
