using Falcon.Core.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Falcon.Api.Infrastructure;

/// <summary>
/// Extension methods for adding an admin user.
/// </summary>
public static class AdminUserExtensions
{
    /// <summary>
    /// Adds an admin user to the system if it does not already exist.
    /// </summary>
    /// <param name="services">The service collection to add the admin user to.</param>
    /// <param name="configuration">The configuration containing admin user settings.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="InvalidOperationException">Thrown if admin user configuration is missing or user creation fails.</exception>
    public static async Task<IServiceCollection> AddAdminUserAsync(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();

        SignInManager<User> signInManager = scope.ServiceProvider.GetRequiredService<
            SignInManager<User>
        >();

        UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<
            UserManager<User>
        >();

        string? name = configuration["Admin:Name"];
        string? email = configuration["Admin:Email"];
        string ra = "000000";
        string? password = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Admin name is not configured.");

        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidOperationException("Admin email is not configured.");

        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Admin password is not configured.");

        User? existentUser = await userManager.FindByNameAsync("admin");

        if (existentUser != null)
            return services;

        User newAdmin = new User(name, email, ra, null, null);

        IdentityResult result = await userManager.CreateAsync(newAdmin, password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Failed to create admin user: "
                    + string.Join(", ", result.Errors.Select(e => e.Description))
            );
        }

        await userManager.AddToRoleAsync(newAdmin, "Admin");

        return services;
    }
}
