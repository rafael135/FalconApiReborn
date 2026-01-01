using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Users;

namespace Falcon.Core.Domain.Auditing;

/// <summary>
/// Represents a log entry in the system for tracking user actions.
/// </summary>
public class Log : Entity
{
    public LogType ActionType { get; private set; }
    public DateTime ActionTime { get; private set; }
    public string IpAddress { get; private set; }

    public string? UserId { get; private set; }
    public virtual User? User { get; private set; }

    public Guid? GroupId { get; private set; }
    public virtual Groups.Group? Group { get; private set; }

    public Guid? CompetitionId { get; private set; }
    public virtual Competitions.Competition? Competition { get; private set; }

#pragma warning disable CS8618
    protected Log() { }
#pragma warning restore CS8618

    public Log(
        LogType actionType,
        string ipAddress,
        User? user = null,
        Groups.Group? group = null,
        Competitions.Competition? competition = null)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address is required");

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
