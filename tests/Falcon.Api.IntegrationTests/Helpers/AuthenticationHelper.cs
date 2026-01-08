using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Falcon.Core.Domain.Users;
using Falcon.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Falcon.Api.IntegrationTests.Helpers;

public static class AuthenticationHelper
{
    public static string GenerateJwtToken(User user, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Email!),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("id", user.Id), // Keep for backward compatibility
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CustomWebApplicationFactory.TestJwtSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: CustomWebApplicationFactory.TestJwtIssuer,
            audience: CustomWebApplicationFactory.TestJwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(5),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static async Task<(User User, string Token)> CreateAuthenticatedUserAsync(
        UserManager<User> userManager,
        string email = "test@example.com",
        string name = "Test User",
        string ra = "123456",
        string password = "password123",
        string role = "Student",
        int? joinYear = null,
        string? department = null)
    {
        var user = new User(name, email, ra, joinYear ?? DateTime.Now.Year, department);
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await userManager.AddToRoleAsync(user, role);

        var token = GenerateJwtToken(user, role);
        return (user, token);
    }

    public static async Task<(User User, string Token)> CreateAuthenticatedStudentAsync(
        UserManager<User> userManager,
        string email = "student@example.com",
        string name = "Test Student",
        string ra = "111111",
        string password = "password123")
    {
        return await CreateAuthenticatedUserAsync(userManager, email, name, ra, password, "Student");
    }

    public static async Task<(User User, string Token)> CreateAuthenticatedTeacherAsync(
        UserManager<User> userManager,
        string email = "teacher@example.com",
        string name = "Test Teacher",
        string ra = "222222",
        string password = "password123")
    {
        return await CreateAuthenticatedUserAsync(userManager, email, name, ra, password, "Teacher");
    }

    public static async Task<(User User, string Token)> CreateAuthenticatedAdminAsync(
        UserManager<User> userManager,
        string email = "admin@example.com",
        string name = "Test Admin",
        string ra = "333333",
        string password = "password123")
    {
        return await CreateAuthenticatedUserAsync(userManager, email, name, ra, password, "Admin");
    }
}
