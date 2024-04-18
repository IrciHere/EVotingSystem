namespace EVotingSystem.Contracts.Vote;

public class VoteDto
{
    public int VotedCandidateId { get; init; }
    public int ElectionId { get; init; }
    public byte[] VoteHash { get; init; }
}