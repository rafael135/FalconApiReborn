using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.BlockGroup;

/// <summary>
/// Handler for blocking a group from submitting in a competition.
/// </summary>
public class BlockGroupHandler : IRequestHandler<BlockGroupCommand, BlockGroupResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<BlockGroupHandler> _logger;

    public BlockGroupHandler(
        FalconDbContext dbContext,
        ILogger<BlockGroupHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

/// <summary>
    /// Handles the <see cref="BlockGroupCommand"/> and blocks a group's participation in the competition.
    /// </summary>
    /// <param name="request">Command containing competition and group ids.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="BlockGroupResult"/> indicating success or failure.</returns>
    /// <exception cref="FormException">Thrown when the group is not registered in the competition.</exception>
    public async Task<BlockGroupResult> Handle(BlockGroupCommand request, CancellationToken cancellationToken)
    {
        // Find group registration
        var registration = await _dbContext.GroupsInCompetitions
            .FirstOrDefaultAsync(g => g.GroupId == request.GroupId && g.CompetitionId == request.CompetitionId, cancellationToken);

        if (registration == null)
        {
            var errors = new Dictionary<string, string>
            {
                { "registration", "Grupo não está inscrito nesta competição" }
            };
            throw new FormException(errors);
        }

        // Call domain method to block
        registration.Block();

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Group {GroupId} blocked in competition {CompetitionId}", 
            request.GroupId, request.CompetitionId);

        return new BlockGroupResult(true, "Grupo bloqueado com sucesso");
    } 
}
