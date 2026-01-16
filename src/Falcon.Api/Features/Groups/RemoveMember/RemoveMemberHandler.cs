using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Groups.RemoveMember;

/// <summary>
/// Handler for removing a member from a group.
/// </summary>
public class RemoveMemberHandler : IRequestHandler<RemoveMemberCommand, RemoveMemberResult>
{
    private readonly UserManager<Core.Domain.Users.User> _userManager;
    private readonly FalconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RemoveMemberHandler> _logger;

    public RemoveMemberHandler(
        UserManager<Core.Domain.Users.User> userManager,
        FalconDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<RemoveMemberHandler> logger
    )
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<RemoveMemberResult> Handle(
        RemoveMemberCommand request,
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

        // Get group with users
        var group = await _dbContext
            .Groups.Include(g => g.Users)
            .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

        if (group == null)
        {
            throw new NotFoundException("Group", request.GroupId);
        }

        // Verify that current user is the leader
        if (group.LeaderId != currentUserId)
        {
            throw new UnauthorizedAccessException("Apenas o líder do grupo pode remover membros");
        }

        // Get target user
        var targetUser = await _userManager.Users.FirstOrDefaultAsync(
            u => u.Id == request.UserId,
            cancellationToken
        );

        if (targetUser == null)
        {
            var errors = new Dictionary<string, string> { { "userId", "Usuário não encontrado" } };
            throw new FormException(errors);
        }

        // Verify that target user is in this group
        if (targetUser.GroupId != group.Id)
        {
            var errors = new Dictionary<string, string>
            {
                { "userId", "Usuário não está neste grupo" },
            };
            throw new FormException(errors);
        }

        // Verify that target user is not the leader
        if (targetUser.Id == group.LeaderId)
        {
            var errors = new Dictionary<string, string>
            {
                { "userId", "Não é possível remover o líder do grupo" },
            };
            throw new FormException(errors);
        }

        // Remove member
        group.RemoveMember(targetUser);
        targetUser.LeaveGroup();

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "User {UserId} removed from group {GroupId} by leader {LeaderId}",
            request.UserId,
            request.GroupId,
            currentUserId
        );

        return new RemoveMemberResult(true, "Membro removido com sucesso");
    }
}
