namespace EVotingSystem.Contracts.Vote;

public class InputVoteDto
{
    public int ElectionId { get; init; }
    public int CandidateId { get; init; }
    public string VoterPassword { get; init; }
}