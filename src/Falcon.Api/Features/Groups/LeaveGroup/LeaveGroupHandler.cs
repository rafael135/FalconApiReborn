using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Groups.LeaveGroup;

/// <summary>
/// Handler for a user leaving their group.
/// </summary>
public class LeaveGroupHandler : IRequestHandler<LeaveGroupCommand, LeaveGroupResult>
{
    private readonly UserManager<Core.Domain.Users.User> _userManager;
    private readonly FalconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<LeaveGroupHandler> _logger;

    public LeaveGroupHandler(
        UserManager<Core.Domain.Users.User> userManager,
        FalconDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<LeaveGroupHandler> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<LeaveGroupResult> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
    {
        // Get logged-in user
        var httpContext = _httpContextAccessor.HttpContext 
            ?? throw new UnauthorizedAccessException("Usuário não autenticado");

        var userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("ID do usuário não encontrado");

        var user = await _userManager.Users
            .Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Usuário não encontrado");

        // Validate user is in a group
        if (user.GroupId == null || user.Group == null)
        {
            var errors = new Dictionary<string, string> { { "user", "Usuário não está em um grupo" } };
            throw new FormException(errors);
        }

        var group = user.Group;

        // Validate user is not the leader
        if (group.LeaderId == userId)
        {
            var errors = new Dictionary<string, string> 
            { 
                { "user", "O líder não pode sair do grupo. Transfira a liderança ou delete o grupo primeiro" } 
            };
            throw new FormException(errors);
        }

        // Remove user from group
        group.RemoveMember(user);
        user.LeaveGroup();

        // Create log entry
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var log = new Falcon.Core.Domain.Auditing.Log(
            LogType.LeaveGroup,
            ipAddress,
            user,
            group,
            null
        );

        await _dbContext.Logs.AddAsync(log, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} left group {GroupId}", userId, group.Id);

        return new LeaveGroupResult(true, "Você saiu do grupo com sucesso");
    }
}
