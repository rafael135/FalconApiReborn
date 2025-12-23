using Falcon.Core.Domain.Users;
using Falcon.Infrastructure.Database;
using MassTransit;
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
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<FalconDbContext>(options => options.UseSqlServer(connectionString));

        services
            .AddIdentityCore<User>(options =>
            {
                // Configurações opcionais de senha/usuario
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>() // Se for usar Roles
            .AddEntityFrameworkStores<FalconDbContext>() // Conecta o Identity ao seu DbContext
            .AddSignInManager<SignInManager<User>>() // Necessário para injeção de SignInManager
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

        return services;
    }
}
