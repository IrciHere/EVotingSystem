namespace EVotingSystem.Database.Entities;

public class VotesOtp
{
    public int VoteId { get; set; }

    public string OtpCode { get; set; }

    public virtual ElectionVote Vote { get; set; }
}
