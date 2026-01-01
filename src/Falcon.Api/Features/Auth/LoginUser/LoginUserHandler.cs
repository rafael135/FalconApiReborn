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
        await this._userManager.UpdateAsync(existentUser);

        // 4. Busca Rica (Operação de Leitura Otimizada)
        // Aqui substituímos aquele repositório genérico complexo.
        // Usamos "Filtered Include" para trazer apenas convites não aceitos de uma vez só.
        var userWithData = await _dbContext
            .Users.AsNoTracking() // Leitura mais rápida (não precisa trackear pois só vamos ler para devolver)
            .Include(u => u.Group)
            .ThenInclude(g => g.Users) // Membros do grupo
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

        return MapToResult(finalUser, role, token);
    }

    private LoginUserResult MapToResult(User user, string role, string token)
    {
        var groupDto =
            user.Group != null
                ? new GroupDto(
                    user.Group.Id,
                    user.Group.Name,
                    user.Group.LeaderId,
                    user.Group.Invites.Select(i => new GroupInviteDto(
                            i.Id,
                            i.UserId,
                            i.GroupId,
                            i.Accepted
                        ))
                        .ToList()
                )
                : null;

        var userDto = new UserDto(user.Id, user.Name, user.Email!, user.RA, role, groupDto);

        return new LoginUserResult(userDto, token);
    }
}
