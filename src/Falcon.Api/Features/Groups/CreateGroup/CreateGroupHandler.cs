using Falcon.Api.Features.Groups.Shared;
using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Users;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Infrastructure.Database;
using Falcon.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Groups.CreateGroup;

/// <summary>
/// Handler for creating a new group.
/// </summary>
public class CreateGroupHandler : IRequestHandler<CreateGroupCommand, CreateGroupResult>
{
    private readonly UserManager<User> _userManager;
    private readonly FalconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CreateGroupHandler> _logger;

    public CreateGroupHandler(
        UserManager<User> userManager,
        FalconDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<CreateGroupHandler> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<CreateGroupResult> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(nameof(request.Name), "Nome é obrigatório");
        else if (request.Name.Length > 100)
            errors.Add(nameof(request.Name), "Nome deve ter no máximo 100 caracteres");

        if (errors.Any())
            throw new FormException(errors);

        // Get logged-in user
        var httpContext = _httpContextAccessor.HttpContext 
            ?? throw new UnauthorizedAccessException("Usuário não autenticado");

        var userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("ID do usuário não encontrado");

        var user = await _userManager.Users
            .Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Usuário não encontrado");

        // Validate user is not already in a group
        if (user.GroupId != null)
        {
            errors.Add("user", "Usuário já está em um grupo");
            throw new FormException(errors);
        }

        // Create group with user as leader
        var group = new Group(request.Name, user);

        await _dbContext.Groups.AddAsync(group, cancellationToken);

        // Create log entry
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var log = new Falcon.Core.Domain.Auditing.Log(
            LogType.CreateGroup,
            ipAddress,
            user,
            group,
            null
        );

        await _dbContext.Logs.AddAsync(log, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Group {GroupId} created by user {UserId}", group.Id, user.Id);

        // Map to DTO
        var groupDto = new GroupDto(
            group.Id,
            group.Name,
            group.LeaderId,
            new List<UserSummaryDto>
            {
                new UserSummaryDto(
                    user.Id,
                    user.Name,
                    user.Email!,
                    user.RA,
                    user.JoinYear,
                    user.Department
                )
            },
            new List<GroupInviteDto>()
        );

        return new CreateGroupResult(groupDto);
    }
}
