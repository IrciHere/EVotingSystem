namespace EVotingSystem.Contracts.Vote;

public class ValidateVoteDto
{
    public int ElectionId { get; init; }
    public byte[] VoteHash { get; init; }
    public string OtpCode { get; init; }
}