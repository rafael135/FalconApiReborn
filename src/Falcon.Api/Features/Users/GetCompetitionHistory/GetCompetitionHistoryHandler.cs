using System.Security.Claims;
using Falcon.Api.Features.Users.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Users.GetCompetitionHistory;

/// <summary>
/// Handler for retrieving a user's competition participation history.
/// Queries historical participation data and aggregates statistics from multiple sources.
/// </summary>
public class GetCompetitionHistoryHandler
    : IRequestHandler<GetCompetitionHistoryQuery, GetCompetitionHistoryResult>
{
    private readonly UserManager<Core.Domain.Users.User> _userManager;
    private readonly FalconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GetCompetitionHistoryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCompetitionHistoryHandler"/> class.
    /// </summary>
    /// <param name="userManager">The user manager for accessing user data.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor for authorization checks.</param>
    /// <param name="logger">The logger instance.</param>
    public GetCompetitionHistoryHandler(
        UserManager<Core.Domain.Users.User> userManager,
        FalconDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<GetCompetitionHistoryHandler> logger
    )
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Handles the get competition history query.
    /// </summary>
    /// <param name="request">The query containing the user ID and pagination parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user's competition history with pagination information.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authorized to access the history.</exception>
    /// <exception cref="NotFoundException">Thrown when the user is not found.</exception>
    public async Task<GetCompetitionHistoryResult> Handle(
        GetCompetitionHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        // Get current user ID from claims
        var httpContext =
            _httpContextAccessor.HttpContext
            ?? throw new UnauthorizedAccessException("Usuário não autenticado");

        var currentUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = httpContext.User.IsInRole("Admin");

        // Authorization: self or admin only
        if (currentUserId != request.UserId && !isAdmin)
        {
            throw new UnauthorizedAccessException("Acesso negado");
        }

        // Verify user exists
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException(nameof(Core.Domain.Users.User), request.UserId);
        }

        // Query UserCompetitionParticipation for historical tracking
        var participations = await _dbContext
            .UserCompetitionParticipations.AsNoTracking()
            .Where(ucp => ucp.UserId == request.UserId)
            .Include(ucp => ucp.Competition)
            .Include(ucp => ucp.Group)
            .OrderByDescending(ucp => ucp.Competition.StartTime)
            .ToListAsync(cancellationToken);

        var historyList = new List<CompetitionHistoryDto>();

        foreach (var participation in participations)
        {
            var competition = participation.Competition;
            var group = participation.Group;

            // Get ranking for this group in this competition
            var ranking = await _dbContext
                .CompetitionRankings.AsNoTracking()
                .FirstOrDefaultAsync(
                    r => r.CompetitionId == competition.Id && r.GroupId == group.Id,
                    cancellationToken
                );

            // Get submission stats for this group in this competition
            var attempts = await _dbContext
                .GroupExerciseAttempts.AsNoTracking()
                .Where(a => a.CompetitionId == competition.Id && a.GroupId == group.Id)
                .ToListAsync(cancellationToken);

            var exercisesSolved = attempts
                .Where(a => a.Accepted)
                .Select(a => a.ExerciseId)
                .Distinct()
                .Count();

            historyList.Add(
                new CompetitionHistoryDto(
                    competition.Id,
                    competition.Name,
                    competition.StartTime,
                    competition.EndTime,
                    competition.Status.ToString(),
                    group.Id,
                    group.Name,
                    ranking?.RankOrder,
                    ranking?.Points ?? 0,
                    ranking?.Penalty ?? 0,
                    exercisesSolved,
                    attempts.Count
                )
            );
        }

        // Apply pagination
        var total = historyList.Count;
        var paginatedHistory = historyList.Skip(request.Skip).Take(request.Take).ToList();

        _logger.LogInformation(
            "Retrieved {Count} competition history records for user {UserId}",
            total,
            request.UserId
        );

        return new GetCompetitionHistoryResult(paginatedHistory, total, request.Skip, request.Take);
    }
}
