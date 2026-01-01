using Falcon.Api.Features.Groups.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Groups.GetGroup;

/// <summary>
/// Handler for retrieving a group by ID.
/// </summary>
public class GetGroupHandler : IRequestHandler<GetGroupQuery, GetGroupResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<GetGroupHandler> _logger;

    public GetGroupHandler(
        FalconDbContext dbContext,
        ILogger<GetGroupHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<GetGroupResult> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        var group = await _dbContext.Groups
            .AsNoTracking()
            .Include(g => g.Users)
            .Include(g => g.Invites.Where(i => !i.Accepted))
                .ThenInclude(i => i.User)
            .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

        if (group == null)
        {
            throw new NotFoundException("Group", request.GroupId);
        }

        _logger.LogInformation("Group {GroupId} retrieved", group.Id);

        // Map to DTO
        var membersDto = group.Users.Select(u => new UserSummaryDto(
            u.Id,
            u.Name,
            u.Email!,
            u.RA,
            u.JoinYear,
            u.Department
        )).ToList();

        var invitesDto = group.Invites.Select(i => new GroupInviteDto(
            i.Id,
            i.UserId,
            i.GroupId,
            i.Accepted
        )).ToList();

        var groupDetailDto = new GroupDetailDto(
            group.Id,
            group.Name,
            group.LeaderId,
            membersDto,
            invitesDto,
            null // LastCompetitionDate will be calculated when competitions are implemented
        );

        return new GetGroupResult(groupDetailDto);
    }
}
