using Falcon.Core.Domain.Groups;

namespace Falcon.Core.Domain.Competitions;

public class GroupInCompetition
{
    public Guid GroupId { get; private set; }
    public virtual Group? Group { get; private set; }

    public Guid CompetitionId { get; private set; }
    public virtual Competition? Competition { get; private set; }

    public DateTime CreatedOn { get; private set; }
    public bool Blocked { get; private set; }

#pragma warning disable CS8618
    protected GroupInCompetition() { }
#pragma warning restore CS8618

    public GroupInCompetition(Group group, Competition competition)
    {
        if (group == null)
            throw new ArgumentNullException(nameof(group));
        if (competition == null)
            throw new ArgumentNullException(nameof(competition));

        Group = group;
        GroupId = group.Id;
        Competition = competition;
        CompetitionId = competition.Id;
        CreatedOn = DateTime.UtcNow;
        Blocked = false;
    }

    public void Block()
    {
        Blocked = true;
    }

    public void Unblock()
    {
        Blocked = false;
    }
}
