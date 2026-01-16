using Falcon.Api.Features.Submissions.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Domain.Users;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Submissions.GetGroupAttempts;

/// <summary>
/// Handler responsável por buscar as tentativas do grupo do usuário autenticado.
/// </summary>
public class GetGroupAttemptsHandler
    : IRequestHandler<GetGroupAttemptsQuery, GetGroupAttemptsResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetGroupAttemptsHandler(
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
    /// Executa a consulta com filtros opcionais e retorna a lista de tentativas do grupo.
    /// </summary>
    /// <param name="request">Query com filtros (competitionId, exerciseId).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de tentativas do grupo encapsulada em <see cref="GetGroupAttemptsResult"/>.</returns>
    /// <exception cref="FormException">Quando o usuário não faz parte de nenhum grupo.</exception>
    public async Task<GetGroupAttemptsResult> Handle(
        GetGroupAttemptsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
            throw new UnauthorizedAccessException("Usuário não autenticado");

        var user = await _userManager
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);

        if (user?.GroupId == null)
        {
            var errors = new Dictionary<string, string>
            {
                { "group", "Você deve estar em um grupo" },
            };
            throw new FormException(errors);
        }

        IQueryable<Core.Domain.Exercises.GroupExerciseAttempt> query = _dbContext
            .GroupExerciseAttempts.AsNoTracking()
            .Include(a => a.Exercise)
            .Include(a => a.Group)
            .Where(a => a.GroupId == user.GroupId);

        if (request.CompetitionId.HasValue)
            query = query.Where(a => a.CompetitionId == request.CompetitionId.Value);

        if (request.ExerciseId.HasValue)
            query = query.Where(a => a.ExerciseId == request.ExerciseId.Value);

        var attempts = await query
            .OrderByDescending(a => a.SubmissionTime)
            .Select(a => new AttemptDto(
                a.Id,
                a.ExerciseId,
                a.Exercise.Title,
                a.GroupId,
                a.Group.Name,
                a.SubmissionTime,
                a.Language,
                a.Accepted,
                a.JudgeResponse,
                a.Time
            ))
            .ToListAsync(cancellationToken);

        return new GetGroupAttemptsResult(attempts);
    }
}
