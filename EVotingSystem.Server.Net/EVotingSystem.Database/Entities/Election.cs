namespace EVotingSystem.Database.Entities;

public class Election
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
    
    public bool HasEnded { get; set; }

    public virtual ICollection<ElectionResult> ElectionResults { get; set; } = new List<ElectionResult>();

    public virtual ElectionSecret ElectionSecret { get; set; }

    public virtual ICollection<ElectionVote> ElectionVotes { get; set; } = new List<ElectionVote>();

    public virtual ICollection<EligibleVoter> EligibleVoters { get; set; } = new List<EligibleVoter>();

    public virtual ICollection<User> Candidates { get; set; } = new List<User>();
}
