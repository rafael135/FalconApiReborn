using Falcon.Api.Features.Auth.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Domain.Users;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Database;
using Falcon.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Auth.LoginUser;

/// <summary>
/// Handler responsável pela autenticação de usuários.
/// </summary>
public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    private readonly UserManager<User> _userManager;

    private readonly ITokenService _tokenService; // Shared JWT service
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<LoginUserHandler> _logger;

    // The SignInManager can be used, but for Stateless APIs (JWT) we usually don't need to "log in" the Identity session cookie, just validate the password.
    private readonly SignInManager<User> _signInManager;

    public LoginUserHandler(
        UserManager<User> userManager,
        ITokenService tokenService,
        SignInManager<User> signInManager,
        FalconDbContext dbContext,
        ILogger<LoginUserHandler> logger
    )
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Processa o comando de autenticação e retorna o usuário (com dados adicionais) e token.
    /// </summary>
    /// <param name="request">Comando contendo RA/email e senha.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado com `UserDto` e token JWT.</returns>
    /// <exception cref="FormException">Quando o RA/senha forem inválidos.</exception>
    public async Task<LoginUserResult> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken
    )
    {
        User? existentUser = await this._dbContext.Users.FirstOrDefaultAsync(
            u => u.RA == request.RA,
            cancellationToken
        );

        if (existentUser == null)
        {
            throw new FormException(
                new Dictionary<string, string> { { "form", "RA e/ou senha incorreto(s)" } }
            );
        }

        SignInResult result = await this._signInManager.CheckPasswordSignInAsync(
            existentUser,
            request.Password,
            false
        );

        if (result.Succeeded == false)
        {
            throw new FormException(
                new Dictionary<string, string> { { "form", "RA e/ou senha incorreto(s)" } }
            );
        }

        existentUser.UpdateLastLogin();
        // Use DbContext directly to avoid concurrency issues with UserManager
        _dbContext.Users.Update(existentUser);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 4. Busca Rica (Operação de Leitura Otimizada)
        // Aqui substituímos aquele repositório genérico complexo.
        // Usamos "Filtered Include" para trazer apenas convites não aceitos de uma vez só.
        var userWithData = await _dbContext
            .Users.AsNoTracking() // Leitura mais rápida (não precisa trackear pois só vamos ler para devolver)
            .Include(u => u.Group)
            .ThenInclude(g =>
                g.Invites.Where(i => !i.Accepted && i.UserId.ToString() == existentUser.Id)
            ) // <--- FILTRO DIRETO NO INCLUDE!
            .ThenInclude(gi => gi.User)
            .FirstOrDefaultAsync(u => u.Id == existentUser.Id, cancellationToken);

        // Se por algum motivo bizarro o userWithData for null (concorrência extrema), usamos o user básico
        var finalUser = userWithData ?? existentUser;

        // 5. Obter Role
        var roles = await _userManager.GetRolesAsync(finalUser);
        var role = roles.FirstOrDefault() ?? "Student";

        // 6. Gerar Token
        var token = _tokenService.GenerateUserToken(finalUser, role);

        return await MapToResultAsync(finalUser, role, token, cancellationToken);
    }

    private async Task<LoginUserResult> MapToResultAsync(User user, string role, string token, CancellationToken cancellationToken)
    {
        // Load invites separately if group exists
        var groupInvites = user.Group != null
            ? await _dbContext.GroupInvites
                .AsNoTracking()
                .Where(i => i.GroupId == user.Group.Id && !i.Accepted && i.UserId == user.Id)
                .Select(i => new GroupInviteDto(i.Id, i.UserId, i.GroupId, i.Accepted))
                .ToListAsync()
            : new List<GroupInviteDto>();

        var groupDto =
            user.Group != null
                ? new GroupDto(
                    user.Group.Id,
                    user.Group.Name,
                    user.Group.LeaderId,
                    groupInvites
                )
                : null;

        var userDto = new UserDto(user.Id, user.Name, user.Email!, user.RA, role, groupDto);

        return new LoginUserResult(userDto, token);
    }
}
