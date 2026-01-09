using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Groups;

namespace Falcon.Core.Domain.Users;

/// <summary>
/// Represents a user's participation in a competition through a specific group.
/// Tracks historical group membership for competition participation tracking.
/// </summary>
public class UserCompetitionParticipation : Entity
{
    /// <summary>
    /// Gets the ID of the user who participated.
    /// </summary>
    public string UserId { get; private set; } = null!;

    /// <summary>
    /// Gets the ID of the competition the user participated in.
    /// </summary>
    public Guid CompetitionId { get; private set; }

    /// <summary>
    /// Gets the ID of the group the user participated with.
    /// </summary>
    public Guid GroupId { get; private set; }

    /// <summary>
    /// Gets the date and time when the user joined the competition.
    /// </summary>
    public DateTime JoinedAt { get; private set; }

    /// <summary>
    /// Gets the user who participated.
    /// </summary>
    public virtual User User { get; private set; } = null!;

    /// <summary>
    /// Gets the competition the user participated in.
    /// </summary>
    public virtual Competition Competition { get; private set; } = null!;

    /// <summary>
    /// Gets the group the user participated with.
    /// </summary>
    public virtual Group Group { get; private set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserCompetitionParticipation"/> class.
    /// Required by EF Core.
    /// </summary>
    private UserCompetitionParticipation() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserCompetitionParticipation"/> class.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="competitionId">The ID of the competition.</param>
    /// <param name="groupId">The ID of the group.</param>
    public UserCompetitionParticipation(string userId, Guid competitionId, Guid groupId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID não pode ser vazio", nameof(userId));
        if (competitionId == Guid.Empty)
            throw new ArgumentException("Competition ID inválido", nameof(competitionId));
        if (groupId == Guid.Empty)
            throw new ArgumentException("Group ID inválido", nameof(groupId));

        UserId = userId;
        CompetitionId = competitionId;
        GroupId = groupId;
        JoinedAt = DateTime.UtcNow;
    }
}
