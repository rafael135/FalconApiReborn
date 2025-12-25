using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Falcon.Core.Domain.Users;
using Falcon.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Falcon.Infrastructure.Auth;

public class TokenService : ITokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    private readonly IMemoryCache _memoryCache;

    private const string PrivateTokenKey = "privateAccessToken";
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        IConfiguration configuration,
        IMemoryCache memoryCache,
        ILogger<TokenService> logger
    )
    {
        _secretKey =
            configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("Jwt:SecretKey");
        _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
        _audience =
            configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public string? FetchPrivateAccessToken()
    {
        if (this._memoryCache.TryGetValue(PrivateTokenKey, out string? token))
        {
            return token!;
        }

        return null;
    }

    /// <inheritdoc/>
    public string GenerateUserToken(User user, string role)
    {
        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Email!),
            new Claim("id", user.Id),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        ];

        // Chave a ser usada para codificar o token:
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._secretKey));

        var signInCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddDays(5),
            claims: claims,
            issuer: this._issuer,
            audience: this._audience,
            signingCredentials: signInCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc/>
    public string GenerateTeacherRoleInviteToken(TimeSpan expiration)
    {
        Claim[] claims =
        [
            new Claim("Invite", "true"),
            new Claim(ClaimTypes.Role, "Teacher"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._secretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: this._issuer,
            audience: this._audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(expiration),
            signingCredentials: creds
        );

        string generatedToken = new JwtSecurityTokenHandler().WriteToken(token);

        this._memoryCache.Set(PrivateTokenKey, generatedToken);

        return generatedToken;
    }

    /// <inheritdoc />
    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(this._secretKey);

        try
        {
            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = this._issuer,
                    ValidateAudience = true,
                    ValidAudience = this._audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                },
                out SecurityToken validatedToken
            );

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Invalid JWT token.");
            return false;
        }
    }

    /// <inheritdoc />
    public string GenerateJudgeToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._secretKey));

        Claim[] claims = [new Claim("sub", this._secretKey)];

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddDays(1),
            claims: claims,
            issuer: this._issuer,
            audience: this._audience,
            signingCredentials: creds
        );

        return tokenHandler.WriteToken(token);
    }
}
