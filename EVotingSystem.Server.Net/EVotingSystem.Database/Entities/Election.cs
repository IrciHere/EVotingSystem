namespace EVotingSystem.Database.Entities;

public class Election
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public virtual ICollection<ElectionResult> ElectionResults { get; set; }

    public virtual ElectionSecret ElectionSecret { get; set; }

    public virtual ICollection<ElectionVote> ElectionVotes { get; set; }

    public virtual ICollection<User> Users { get; set; }

    public virtual ICollection<User> UsersNavigation { get; set; }
}
