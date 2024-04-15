namespace EVotingSystem.Contracts.Vote;

public class ValidateVoteDto
{
    public int ElectionId { get; set; }
    public byte[] VoteHash { get; set; }
    public string OtpCode { get; set; }
}