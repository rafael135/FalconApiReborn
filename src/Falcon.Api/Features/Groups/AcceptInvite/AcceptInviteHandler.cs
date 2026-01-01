using Falcon.Api.Features.Groups.Shared;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Groups.AcceptInvite;

/// <summary>
/// Handler for accepting a group invitation.
/// </summary>
public class AcceptInviteHandler : IRequestHandler<AcceptInviteCommand, AcceptInviteResult>
{
    private readonly UserManager<Core.Domain.Users.User> _userManager;
    private readonly FalconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AcceptInviteHandler> _logger;

    public AcceptInviteHandler(
        UserManager<Core.Domain.Users.User> userManager,
        FalconDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AcceptInviteHandler> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<AcceptInviteResult> Handle(AcceptInviteCommand request, CancellationToken cancellationToken)
    {
        // Get logged-in user
        var httpContext = _httpContextAccessor.HttpContext 
            ?? throw new UnauthorizedAccessException("Usuário não autenticado");

        var userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("ID do usuário não encontrado");

        // Get invite with group and users
        var invite = await _dbContext.GroupInvites
            .Include(i => i.Group)
                .ThenInclude(g => g.Users)
            .Include(i => i.Group.Invites.Where(inv => !inv.Accepted))
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.Id == request.InviteId, cancellationToken);

        if (invite == null)
        {
            throw new NotFoundException("GroupInvite", request.InviteId);
        }

        // Verify that the invite is for the logged-in user
        if (invite.UserId != userId)
        {
            throw new UnauthorizedAccessException("Este convite não é para você");
        }

        // Verify that invite hasn't been accepted already
        if (invite.Accepted)
        {
            var errors = new Dictionary<string, string> 
            { 
                { "invite", "Convite já foi aceito" } 
            };
            throw new FormException(errors);
        }

        // Get user with group info
        var user = await _userManager.Users
            .Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User", userId);
        }

        // Verify that user is not already in a group
        if (user.GroupId != null)
        {
            var errors = new Dictionary<string, string> 
            { 
                { "user", "Você já está em um grupo" } 
            };
            throw new FormException(errors);
        }

        var group = invite.Group;

        // Validate max members rule again (current members + 1)
        if (group.Users.Count >= 3)
        {
            var errors = new Dictionary<string, string> 
            { 
                { "group", "Grupo já atingiu o número máximo de membros" } 
            };
            throw new FormException(errors);
        }

        // Accept invite
        invite.MarkAsAccepted();
        group.AddMember(user);
        user.AssignGroup(group);

        // Create log entry
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var log = new Falcon.Core.Domain.Auditing.Log(
            LogType.JoinGroup,
            ipAddress,
            user,
            group,
            null
        );

        await _dbContext.Logs.AddAsync(log, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} accepted invite {InviteId} and joined group {GroupId}", 
            userId, request.InviteId, group.Id);

        // Map to DTO
        var membersDto = group.Users.Select(u => new UserSummaryDto(
            u.Id,
            u.Name,
            u.Email!,
            u.RA,
            u.JoinYear,
            u.Department
        )).ToList();

        var invitesDto = group.Invites
            .Where(i => !i.Accepted)
            .Select(i => new GroupInviteDto(
                i.Id,
                i.UserId,
                i.GroupId,
                i.Accepted
            )).ToList();

        var groupDto = new GroupDto(
            group.Id,
            group.Name,
            group.LeaderId,
            membersDto,
            invitesDto
        );

        return new AcceptInviteResult(groupDto);
    }
}
