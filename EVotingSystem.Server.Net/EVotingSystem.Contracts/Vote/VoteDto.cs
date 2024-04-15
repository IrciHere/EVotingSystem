namespace EVotingSystem.Contracts.Vote;

public class VoteDto
{
    public int VotedCandidateId { get; set; }
    public int ElectionId { get; set; }
    public byte[] VoteHash { get; set; }
}