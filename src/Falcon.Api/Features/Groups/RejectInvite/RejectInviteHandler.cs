using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Groups.RejectInvite;

/// <summary>
/// Handler for rejecting a group invitation.
/// </summary>
public class RejectInviteHandler : IRequestHandler<RejectInviteCommand, RejectInviteResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RejectInviteHandler> _logger;

    public RejectInviteHandler(
        FalconDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<RejectInviteHandler> logger)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<RejectInviteResult> Handle(RejectInviteCommand request, CancellationToken cancellationToken)
    {
        // Get logged-in user
        var httpContext = _httpContextAccessor.HttpContext 
            ?? throw new UnauthorizedAccessException("Usuário não autenticado");

        var userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("ID do usuário não encontrado");

        // Get invite
        var invite = await _dbContext.GroupInvites
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

        // Remove invite
        _dbContext.GroupInvites.Remove(invite);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} rejected invite {InviteId}", userId, request.InviteId);

        return new RejectInviteResult(true, "Convite rejeitado com sucesso");
    }
}
