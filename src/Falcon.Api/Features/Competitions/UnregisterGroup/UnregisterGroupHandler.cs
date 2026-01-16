using Falcon.Core.Domain.Auditing;
using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.UnregisterGroup;

/// <summary>
/// Handler for unregistering a group from a competition.
/// </summary>
public class UnregisterGroupHandler : IRequestHandler<UnregisterGroupCommand, UnregisterGroupResult>
{
    private readonly UserManager<Core.Domain.Users.User> _userManager;
    private readonly FalconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UnregisterGroupHandler> _logger;

    public UnregisterGroupHandler(
        UserManager<Core.Domain.Users.User> userManager,
        FalconDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UnregisterGroupHandler> logger
    )
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<UnregisterGroupResult> Handle(
        UnregisterGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        // Get logged-in user
        var httpContext =
            _httpContextAccessor.HttpContext
            ?? throw new UnauthorizedAccessException("Usuário não autenticado");

        var userId =
            httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("ID do usuário não encontrado");

        // Get user with group
        var user = await _userManager
            .Users.Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User", userId);
        }

        // Verify user is group leader
        if (user.Group == null || user.Group.LeaderId != userId)
        {
            throw new UnauthorizedAccessException(
                "Apenas o líder do grupo pode cancelar a inscrição"
            );
        }

        var group = user.Group;

        // Get competition
        var competition = await _dbContext.Competitions.FirstOrDefaultAsync(
            c => c.Id == request.CompetitionId,
            cancellationToken
        );

        if (competition == null)
        {
            throw new NotFoundException("Competition", request.CompetitionId);
        }

        // Verify competition hasn't started yet
        if (
            competition.Status == CompetitionStatus.Ongoing
            || competition.Status == CompetitionStatus.Finished
        )
        {
            var errors = new Dictionary<string, string>
            {
                {
                    "competition",
                    "Não é possível cancelar inscrição em competição já iniciada ou finalizada"
                },
            };
            throw new FormException(errors);
        }

        // Find and remove registration
        var registration = await _dbContext.GroupsInCompetitions.FirstOrDefaultAsync(
            g => g.GroupId == group.Id && g.CompetitionId == request.CompetitionId,
            cancellationToken
        );

        if (registration == null)
        {
            var errors = new Dictionary<string, string>
            {
                { "registration", "Grupo não está inscrito nesta competição" },
            };
            throw new FormException(errors);
        }

        _dbContext.GroupsInCompetitions.Remove(registration);

        // Remove ranking entry
        var ranking = await _dbContext.CompetitionRankings.FirstOrDefaultAsync(
            r => r.GroupId == group.Id && r.CompetitionId == request.CompetitionId,
            cancellationToken
        );

        if (ranking != null)
        {
            _dbContext.CompetitionRankings.Remove(ranking);
        }

        // Create log entry
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var log = new Log(LogType.LeaveCompetition, ipAddress, user, group, competition);
        await _dbContext.Logs.AddAsync(log, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Group {GroupId} unregistered from competition {CompetitionId}",
            group.Id,
            competition.Id
        );

        return new UnregisterGroupResult(true, "Inscrição cancelada com sucesso");
    }
}
