namespace EVotingSystem.Database.Entities;

public class EligibleVoter
{
    public int UserId { get; set; }

    public int ElectionId { get; set; }

    public bool HasVoted { get; set; }

    public virtual Election Election { get; set; }

    public virtual User User { get; set; }
}
