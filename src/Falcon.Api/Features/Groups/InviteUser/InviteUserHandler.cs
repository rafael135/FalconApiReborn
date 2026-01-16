using Falcon.Api.Features.Groups.Shared;
using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Groups.InviteUser;

/// <summary>
/// Handler for inviting a user to a group.
/// </summary>
public class InviteUserHandler : IRequestHandler<InviteUserCommand, InviteUserResult>
{
    private readonly UserManager<Core.Domain.Users.User> _userManager;
    private readonly FalconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<InviteUserHandler> _logger;

    public InviteUserHandler(
        UserManager<Core.Domain.Users.User> userManager,
        FalconDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<InviteUserHandler> logger
    )
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<InviteUserResult> Handle(
        InviteUserCommand request,
        CancellationToken cancellationToken
    )
    {
        // Get logged-in user
        var httpContext =
            _httpContextAccessor.HttpContext
            ?? throw new UnauthorizedAccessException("Usuário não autenticado");

        var currentUserId =
            httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("ID do usuário não encontrado");

        // Get group with users and invites
        var group = await _dbContext
            .Groups.Include(g => g.Users)
            .Include(g => g.Invites)
            .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

        if (group == null)
        {
            throw new NotFoundException("Group", request.GroupId);
        }

        // Verify that current user is the leader
        if (group.LeaderId != currentUserId)
        {
            throw new UnauthorizedAccessException("Apenas o líder do grupo pode convidar usuários");
        }

        // Get target user by RA
        var targetUser = await _userManager
            .Users.Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.RA == request.RA, cancellationToken);

        if (targetUser == null)
        {
            var errors = new Dictionary<string, string>
            {
                { "ra", "Usuário não encontrado com este RA" },
            };
            throw new FormException(errors);
        }

        // Validate target user is not already in a group
        if (targetUser.GroupId != null)
        {
            var errors = new Dictionary<string, string> { { "ra", "Usuário já está em um grupo" } };
            throw new FormException(errors);
        }

        // Check if user already has a pending invite for this group
        var existingInvite = group.Invites.FirstOrDefault(i =>
            i.UserId == targetUser.Id && !i.Accepted
        );
        if (existingInvite != null)
        {
            var errors = new Dictionary<string, string>
            {
                { "ra", "Usuário já tem um convite pendente para este grupo" },
            };
            throw new FormException(errors);
        }

        // Validate max members rule (current members + pending invites + this invite)
        var pendingInvitesCount = group.Invites.Count(i => !i.Accepted);
        var totalCount = group.Users.Count + pendingInvitesCount + 1;

        if (totalCount >= 3)
        {
            var errors = new Dictionary<string, string>
            {
                { "group", "Grupo não pode ter mais de 3 membros (incluindo convites pendentes)" },
            };
            throw new FormException(errors);
        }

        // Create invite
        var invite = new GroupInvite(group, targetUser);

        await _dbContext.GroupInvites.AddAsync(invite, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "User {UserId} invited to group {GroupId} by {LeaderId}",
            targetUser.Id,
            request.GroupId,
            currentUserId
        );

        var inviteDto = new GroupInviteDto(
            invite.Id,
            invite.UserId,
            invite.GroupId,
            invite.Accepted
        );

        return new InviteUserResult(inviteDto);
    }
}
