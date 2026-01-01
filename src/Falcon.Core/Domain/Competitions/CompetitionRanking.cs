namespace Falcon.Core.Domain.Competitions;

/// <summary>
/// Represents the ranking information of a group within a specific competition,
/// including points earned and the order in the ranking.
/// </summary>
public class CompetitionRanking : Entity
{
    public Guid CompetitionId { get; private set; }
    public virtual Competition Competition { get; private set; }

    public Guid GroupId { get; private set; }
    public virtual Groups.Group Group { get; private set; }

    public double Points { get; private set; }
    public double Penalty { get; private set; }
    public int RankOrder { get; private set; }

#pragma warning disable CS8618
    protected CompetitionRanking() { }
#pragma warning restore CS8618

    public CompetitionRanking(Competition competition, Groups.Group group)
    {
        if (competition == null)
            throw new ArgumentNullException(nameof(competition));
        if (group == null)
            throw new ArgumentNullException(nameof(group));

        Competition = competition;
        CompetitionId = competition.Id;
        Group = group;
        GroupId = group.Id;
        Points = 0;
        Penalty = 0;
        RankOrder = 0;
    }

    public void UpdatePoints(double points)
    {
        if (points < 0)
            throw new ArgumentException("Points cannot be negative");
        
        Points = points;
    }

    public void AddPenalty(double penalty)
    {
        if (penalty < 0)
            throw new ArgumentException("Penalty cannot be negative");
        
        Penalty += penalty;
    }

    public void UpdateRankOrder(int order)
    {
        if (order < 1)
            throw new ArgumentException("Rank order must be greater than 0");
        
        RankOrder = order;
    }
}
