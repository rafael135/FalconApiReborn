using Falcon.Api.Features.Groups.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Groups.UpdateGroup;

/// <summary>
/// Handler for updating a group's name.
/// </summary>
public class UpdateGroupHandler : IRequestHandler<UpdateGroupCommand, UpdateGroupResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UpdateGroupHandler> _logger;

    public UpdateGroupHandler(
        FalconDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UpdateGroupHandler> logger)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<UpdateGroupResult> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
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

        // Get group with members
        var group = await _dbContext.Groups
            .Include(g => g.Users)
            .Include(g => g.Invites.Where(i => !i.Accepted))
                .ThenInclude(i => i.User)
            .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

        if (group == null)
        {
            throw new NotFoundException("Group", request.GroupId);
        }

        // Verify that user is the leader
        if (group.LeaderId != userId)
        {
            throw new UnauthorizedAccessException("Apenas o líder do grupo pode alterar seu nome");
        }

        // Update group name
        group.Rename(request.Name);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Group {GroupId} renamed to {GroupName} by user {UserId}", 
            group.Id, request.Name, userId);

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

        var groupDto = new GroupDto(
            group.Id,
            group.Name,
            group.LeaderId,
            membersDto,
            invitesDto
        );

        return new UpdateGroupResult(groupDto);
    }
}
