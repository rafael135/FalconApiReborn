using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Domain.Users;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;

//using ProjetoTccBackend.Exceptions; // Sua FormException
//using ProjetoTccBackend.Services.Interfaces; // Seu ITokenService

namespace Falcon.Api.Features.Auth.RegisterUser;

/// <summary>
/// Handler responsible for processing user registration requests.
/// </summary>
public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly UserManager<User> _userManager;

    private readonly ITokenService _tokenService; // Shared JWT service
    private readonly ILogger<RegisterUserHandler> _logger;

    // The SignInManager can be used, but for Stateless APIs (JWT) we usually don't need to "log in" the Identity session cookie, just validate the password.
    private readonly SignInManager<User> _signInManager;

    public RegisterUserHandler(
        UserManager<User> userManager,
        ITokenService tokenService,
        SignInManager<User> signInManager,
        ILogger<RegisterUserHandler> logger
    )
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
        _logger = logger;
    }

    /// <summary>
    /// Processes the registration command and returns the created user's information with JWT token.
    /// </summary>
    /// <param name="request">The registration command containing user details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="RegisterUserResult"/> containing user information and token.</returns>
    /// <exception cref="FormException">Thrown when validation fails (duplicate email/RA, invalid access code, identity errors).</exception>
    public async Task<RegisterUserResult> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken
    )
    {
        // 1. Email validation (Business Rule)
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
            throw new FormException(
                new Dictionary<string, string> { { "email", "E-mail já utilizado" } }
            );
        }

        // 2. RA validation (Business Rule)
        // Note: If UserManager does not search by RA, here you can inject the DbContext directly and do: _context.Users.AnyAsync(u => u.RA == request.Ra)
        // I will assume you have a method or will do it via LINQ on UserManager.Users
        if (_userManager.Users.Any(u => u.RA == request.Ra))
        {
            _logger.LogWarning("Registration attempt with existing RA: {RA}", request.Ra);
            throw new FormException(
                new Dictionary<string, string> { { "ra", "RA já cadastrado no sistema" } }
            );
        }

        // 3. Role validation (Teacher)
        if (request.Role == "Teacher")
        {
            if (
                string.IsNullOrEmpty(request.AccessCode)
                || !_tokenService.ValidateToken(request.AccessCode)
            )
            {
                throw new FormException(
                    new Dictionary<string, string> { { "accessCode", "Código de acesso inválido" } }
                );
            }
        }

        if (request.Role == "Admin")
        {
            throw new FormException(
                new Dictionary<string, string>
                {
                    { "form", "Não é possível criar Admin por aqui." },
                }
            );
        }

        // 4. Entity Creation
        User newUser = new User(
            request.Name,
            request.Email,
            request.Ra,
            request.joinYear,
            request.Department
        );

        // 5. Persistence
        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            // Error mapping (exactly as you had it)
            var errors = result.Errors.ToFriendlyDictionary();
            throw new FormException(errors);
        }

        // 6. Role Assignment
        await _userManager.AddToRoleAsync(newUser, request.Role);

        // Optional: Automatic login in Identity (creates asp.net core session cookie, if needed)
        // await _signInManager.PasswordSignInAsync(newUser, request.Password, false, false);

        // 7. JWT Token Generation (Now part of the Handler flow to return everything ready)
        string jwtToken = _tokenService.GenerateUserToken(newUser, request.Role);

        // 8. Return Result DTO
        return new RegisterUserResult(
            newUser.Id,
            newUser.Name,
            newUser.Email!,
            newUser.RA,
            request.Role,
            jwtToken,
            newUser.EmailConfirmed,
            newUser.JoinYear
        );
    }
}
