using Falcon.Api.Features.Competitions.Shared;
using Falcon.Core;
using Falcon.Core.Domain.Auditing;
using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Competitions.Rules;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.RegisterGroup;

/// <summary>
/// Handler for registering a group in a competition.
/// </summary>
public class RegisterGroupHandler : IRequestHandler<RegisterGroupCommand, RegisterGroupResult>
{
    private readonly UserManager<Core.Domain.Users.User> _userManager;
    private readonly FalconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RegisterGroupHandler> _logger;

    public RegisterGroupHandler(
        UserManager<Core.Domain.Users.User> userManager,
        FalconDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<RegisterGroupHandler> logger
    )
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<RegisterGroupResult> Handle(
        RegisterGroupCommand request,
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
            .ThenInclude(g => g!.Users)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User", userId);
        }

        // Verify user is in a group
        if (user.Group == null)
        {
            var errors = new Dictionary<string, string>
            {
                { "group", "Você deve estar em um grupo para se inscrever em uma competição" },
            };
            throw new FormException(errors);
        }

        var group = user.Group;

        // Get competition
        var competition = await _dbContext
            .Competitions.Include(c => c.GroupsInCompetitions)
            .FirstOrDefaultAsync(c => c.Id == request.CompetitionId, cancellationToken);

        if (competition == null)
        {
            throw new NotFoundException("Competition", request.CompetitionId);
        }

        // Check business rules using domain entities
        var isAlreadyRegistered = competition.GroupsInCompetitions.Any(g => g.GroupId == group.Id);

        var inscriptionsOpenRule = new InscriptionsMustBeOpenRule(competition.IsInscriptionOpen);
        if (inscriptionsOpenRule.IsBroken())
            throw new BusinessRuleException(inscriptionsOpenRule);

        var alreadyRegisteredRule = new GroupCannotBeAlreadyRegisteredRule(isAlreadyRegistered);
        if (alreadyRegisteredRule.IsBroken())
            throw new BusinessRuleException(alreadyRegisteredRule);

        var requiredMembersRule = new GroupMustHaveRequiredMembersRule(
            group.Users.Count,
            competition.MaxMembers ?? 3
        );
        if (requiredMembersRule.IsBroken())
            throw new BusinessRuleException(requiredMembersRule);

        // Create registration
        var groupInCompetition = new GroupInCompetition(group, competition);
        await _dbContext.GroupsInCompetitions.AddAsync(groupInCompetition, cancellationToken);

        // Create ranking entry
        var ranking = new CompetitionRanking(competition, group);
        await _dbContext.CompetitionRankings.AddAsync(ranking, cancellationToken);

        // Create user competition participation records for all group members
        foreach (var member in group.Users)
        {
            var participation = new Core.Domain.Users.UserCompetitionParticipation(
                member.Id,
                competition.Id,
                group.Id
            );
            await _dbContext.UserCompetitionParticipations.AddAsync(
                participation,
                cancellationToken
            );
        }

        // Create log entry
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var log = new Log(LogType.JoinCompetition, ipAddress, user, group, competition);
        await _dbContext.Logs.AddAsync(log, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Group {GroupId} registered in competition {CompetitionId}",
            group.Id,
            competition.Id
        );

        var registrationDto = new GroupInCompetitionDto(
            groupInCompetition.GroupId,
            groupInCompetition.CompetitionId,
            groupInCompetition.CreatedOn,
            groupInCompetition.Blocked
        );

        return new RegisterGroupResult(registrationDto);
    }
}
