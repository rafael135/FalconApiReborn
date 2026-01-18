using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Users;

namespace Falcon.Core.Domain.Auditing;

/// <summary>
/// Represents a log entry in the system for tracking user actions.
/// </summary>
public class Log : Entity
{
    /// <summary>
    /// The type of action logged.
    /// </summary>
    public LogType ActionType { get; private set; }

    /// <summary>
    /// The timestamp when the action occurred.
    /// </summary>
    public DateTime ActionTime { get; private set; }

    /// <summary>
    /// The IP address from which the action was performed.
    /// </summary>
    public string IpAddress { get; private set; }

    /// <summary>
    /// The user associated with the action, if applicable.
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// The user associated with the action, if applicable.
    /// </summary>
    public virtual User? User { get; private set; }

    /// <summary>
    /// The group associated with the action, if applicable.
    /// </summary>
    public Guid? GroupId { get; private set; }

    /// <summary>
    /// The group associated with the action, if applicable.
    /// </summary>
    public virtual Groups.Group? Group { get; private set; }

    /// <summary>
    /// The competition associated with the action, if applicable.
    /// </summary>
    public Guid? CompetitionId { get; private set; }

    /// <summary>
    /// The competition associated with the action, if applicable.
    /// </summary>
    public virtual Competition? Competition { get; private set; }

    /// <summary>
    /// Required by EF Core.
    /// </summary>
#pragma warning disable CS8618
    protected Log() { }
#pragma warning restore CS8618

    /// <summary>
    /// Creates a new log entry.
    /// </summary>
    /// <param name="actionType">The type of action being logged.</param>
    /// <param name="ipAddress">The IP address from which the action was performed.</param>
    /// <param name="user">The user associated with the action, if applicable.</param>
    /// <param name="group">The group associated with the action, if applicable.</param>
    /// <param name="competition">The competition associated with the action, if applicable.</param>
    /// <exception cref="ArgumentException">If the IP address is null or whitespace.</exception>
    public Log(
        LogType actionType,
        string ipAddress,
        User? user = null,
        Groups.Group? group = null,
        Competition? competition = null
    )
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("Endereço IP é obrigatório");

        ActionType = actionType;
        ActionTime = DateTime.UtcNow;
        IpAddress = ipAddress;

        User = user;
        UserId = user?.Id;

        Group = group;
        GroupId = group?.Id;

        Competition = competition;
        CompetitionId = competition?.Id;
    }
}
