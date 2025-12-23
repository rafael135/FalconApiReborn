using Falcon.Api.Features.Auth.RegisterUser;
using Falcon.Core.Domain; // Suas entidades
using Falcon.Core.Domain.Users;
using Falcon.Infrastructure.Database; // Seu DbContext se precisar queries diretas (opcional aqui pois usamos UserManager)
using MediatR;
using Microsoft.AspNetCore.Identity;

//using ProjetoTccBackend.Exceptions; // Sua FormException
//using ProjetoTccBackend.Services.Interfaces; // Seu ITokenService

namespace Falcon.Api.Features.Auth.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly UserManager<User> _userManager;

    //TODO: Implement Token Service
    //private readonly ITokenService _tokenService; // Serviço compartilhado de JWT
    private readonly ILogger<RegisterUserHandler> _logger;

    // O SignInManager pode ser usado, mas para APIs Stateless (JWT) geralmente não precisamos "logar" o cookie de sessão do Identity, apenas validar a senha.
    // Mas manterei para fidelidade ao seu código original se você usa side-effects dele.
    private readonly SignInManager<User> _signInManager;

    public RegisterUserHandler(
        UserManager<User> userManager,
        //ITokenService tokenService,
        SignInManager<User> signInManager,
        ILogger<RegisterUserHandler> logger
    )
    {
        _userManager = userManager;
        //_tokenService = tokenService;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<RegisterUserResult> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken
    )
    {
        // 1. Validação de Email (Regra de Negócio)
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
            //TODO: Implement FormException
            //throw new FormException(
            //    new Dictionary<string, string> { { "email", "E-mail já utilizado" } }
            //);
        }

        // 2. Validação de RA (Regra de Negócio)
        // Nota: Se UserManager não busca por RA, aqui você pode injetar o DbContext direto e fazer: _context.Users.AnyAsync(u => u.RA == request.Ra)
        // Vou assumir que você tem um método ou fará via LINQ no UserManager.Users
        if (_userManager.Users.Any(u => u.RA == request.Ra))
        {
            _logger.LogWarning("Registration attempt with existing RA: {RA}", request.Ra);
            //throw new FormException(
            //    new Dictionary<string, string> { { "ra", "RA já cadastrado no sistema" } }
            //);
        }

        // 3. Validação de Role (Teacher)
        if (request.Role == "Teacher")
        {
            if (string.IsNullOrEmpty(request.AccessCode)
            //    || !_tokenService.ValidateToken(request.AccessCode)
            )
            {
                //    throw new FormException(
                //        new Dictionary<string, string> { { "accessCode", "Código de acesso inválido" } }
                //    );
            }
        }

        if (request.Role == "Admin")
        {
            //throw new FormException(
            //    new Dictionary<string, string>
            //    {
            //        { "form", "Não é possível criar Admin por aqui." },
            //    }
            //);
        }

        // 4. Criação da Entidade
        User newUser = new User(request.Name, request.Email, request.Ra, request.joinYear);

        // 5. Persistência
        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            // Mapeamento de erros (exatamente como você tinha)
            var errors = MapIdentityErrors(result.Errors);
            //throw new FormException(errors);
        }

        // 6. Atribuição de Role
        await _userManager.AddToRoleAsync(newUser, request.Role);

        // Opcional: Login automático no Identity (cria cookie de sessão asp.net core, se necessário)
        // await _signInManager.PasswordSignInAsync(newUser, request.Password, false, false);

        // 7. Geração do Token JWT (Agora faz parte do fluxo do Handler retornar tudo pronto)
        //var jwtToken = _tokenService.GenerateUserToken(newUser, request.Role);

        // 8. Retorno do DTO de Resultado
        return new RegisterUserResult(
            newUser.Id,
            newUser.Name,
            newUser.Email!,
            newUser.RA,
            request.Role,
            "", //jwtToken,
            newUser.EmailConfirmed,
            newUser.JoinYear
        );
    }

    private Dictionary<string, string> MapIdentityErrors(IEnumerable<IdentityError> errors)
    {
        var errorMessages = new Dictionary<string, string>();
        foreach (var error in errors)
        {
            errorMessages["form"] = error.Description;
        }
        return errorMessages;
    }
}
