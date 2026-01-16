using Falcon.Api.Features.Submissions.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Domain.Users;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Submissions.GetAttempt;

/// <summary>
/// Handler para obter os detalhes de uma tentativa.
/// </summary>
public class GetAttemptHandler : IRequestHandler<GetAttemptQuery, GetAttemptResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAttemptHandler(
        FalconDbContext dbContext,
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Busca uma tentativa por ID, validando permissões de acesso (membro do grupo ou Teacher/Admin).
    /// </summary>
    /// <param name="request">Query com ID da tentativa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Detalhes da tentativa em um <see cref="GetAttemptResult"/>.</returns>
    /// <exception cref="UnauthorizedAccessException">Quando usuário não autenticado ou sem permissão.</exception>
    /// <exception cref="NotFoundException">Quando a tentativa não é encontrada.</exception>
    /// <exception cref="FormException">Quando usuário não pertence ao grupo dono da tentativa.</exception>
    public async Task<GetAttemptResult> Handle(
        GetAttemptQuery request,
        CancellationToken cancellationToken
    )
    {
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
            throw new UnauthorizedAccessException("Usuário não autenticado");

        var user = await _userManager
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("Usuário não encontrado");

        var roles = await _userManager.GetRolesAsync(user);
        var isTeacherOrAdmin = roles.Contains("Teacher") || roles.Contains("Admin");

        var attempt = await _dbContext
            .GroupExerciseAttempts.AsNoTracking()
            .Include(a => a.Exercise)
            .Include(a => a.Group)
            .FirstOrDefaultAsync(a => a.Id == request.AttemptId, cancellationToken);

        if (attempt == null)
            throw new NotFoundException("Attempt", request.AttemptId);

        if (!isTeacherOrAdmin && attempt.GroupId != user.GroupId)
        {
            var errors = new Dictionary<string, string>
            {
                { "attempt", "Você não tem permissão para visualizar esta submissão" },
            };
            throw new FormException(errors);
        }

        var attemptDetail = new AttemptDetailDto(
            attempt.Id,
            attempt.ExerciseId,
            attempt.Exercise.Title,
            attempt.GroupId,
            attempt.Group.Name,
            attempt.Code,
            attempt.Language,
            attempt.SubmissionTime,
            attempt.Time,
            attempt.Accepted,
            attempt.JudgeResponse
        );

        return new GetAttemptResult(attemptDetail);
    }
}
