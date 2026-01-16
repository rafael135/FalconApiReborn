using Falcon.Core.Domain.Users;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Auth;
using Falcon.Infrastructure.Database;
using Falcon.Infrastructure.Files;
using Falcon.Infrastructure.Judge;
using Falcon.Infrastructure.Storage;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Falcon.Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add infrastructure services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="InvalidOperationException">If the FalconDbContext is not configured properly.</exception>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMemoryCache();

        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        string? environment =
            configuration["ASPNETCORE_ENVIRONMENT"]
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? configuration["Environment"]
            ?? "Production";

        // Only register SQL Server if connection string is provided and not in Testing environment
        // This allows tests to register InMemory database without conflicts
        if (
            !string.IsNullOrWhiteSpace(connectionString)
            && environment != "Testing"
            && environment != "Test"
        )
        {
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
        }
        else if (environment != "Testing" && environment != "Test")
        {
            // In non-testing environments, FalconDbContext must be configured.
            // Fail fast with a clear message if the DefaultConnection string is missing or empty.
            throw new InvalidOperationException(
                "FalconDbContext is not configured. Connection string 'DefaultConnection' is missing or empty. "
                    + "Ensure a valid connection string is provided in configuration, or register a DbContext explicitly "
                    + "in AddInfrastructure."
            );
        }
        // If connection string is empty/null or in Testing environment,
        // tests will register InMemory database in ConfigureTestServices

        services.AddDataProtection();

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

        // Add JWT authentication with query string support for SignalR
        if (environment != "Testing" && environment != "Test")
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Bearer";
                    options.DefaultChallengeScheme = "Bearer";
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey =
                                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                                    System.Text.Encoding.UTF8.GetBytes(
                                        configuration["Jwt:SecretKey"] ?? ""
                                    )
                                ),
                            ValidateIssuer = true,
                            ValidIssuer = configuration["Jwt:Issuer"],
                            ValidateAudience = true,
                            ValidAudience = configuration["Jwt:Audience"],
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero,
                        };
                    options.Events =
                        new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["token"];
                                var path = context.HttpContext.Request.Path;
                                if (
                                    !string.IsNullOrEmpty(accessToken)
                                    && path.StartsWithSegments("/hubs/competition")
                                )
                                {
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            },
                        };
                });
        }

        // Configure HttpClient for Judge API
        var judgeApiUrl = configuration["JudgeApi:Url"];
        if (!string.IsNullOrEmpty(judgeApiUrl))
        {
            services
                .AddHttpClient(
                    "JudgeAPI",
                    client =>
                    {
                        client.BaseAddress = new Uri(judgeApiUrl);
                        client.Timeout = TimeSpan.FromSeconds(30);
                    }
                )
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (
                            message,
                            cert,
                            chain,
                            sslPolicyErrors
                        ) => true,
                    }
                );
        }

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IJudgeService, JudgeService>();

        services.AddSingleton<IFileStorageService>(provider =>
        {
            ILogger<LocalFileStorageService> logger = provider.GetRequiredService<
                ILogger<LocalFileStorageService>
            >();
            IWebHostEnvironment? webEnv = provider.GetService<IWebHostEnvironment>();

            if (webEnv != null)
            {
                return new LocalFileStorageService(webEnv, logger);
            }

            IHostEnvironment hostEnv = provider.GetRequiredService<IHostEnvironment>();
            string basePath = Path.Combine(
                hostEnv.ContentRootPath ?? Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads"
            );

            return new LocalFileStorageService(basePath, logger);
        });

        services.AddScoped<IAttachedFileService, AttachedFileService>();

        services.AddHttpContextAccessor();

        return services;
    }

    /// <summary>
    /// Adds MassTransit configuration for API with consumers.
    /// </summary>
    public static IServiceCollection AddApiMassTransit(
        this IServiceCollection services,
        Action<IRegistrationConfigurator> configureConsumers,
        IConfiguration configuration
    )
    {
        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            configureConsumers(bus);

            bus.UsingRabbitMq(
                (context, cfg) =>
                {
                    string host = configuration.GetValue<string>("RabbitMQ:Host", "localhost");
                    string username = configuration.GetValue<string>("RabbitMQ:Username", "guest");
                    string password = configuration.GetValue<string>("RabbitMQ:Password", "guest");
                    cfg.Host(
                        host,
                        "/",
                        h =>
                        {
                            h.Username(username);
                            h.Password(password);
                        }
                    );

                    cfg.ConfigureEndpoints(context);
                }
            );
        });

        return services;
    }
}
