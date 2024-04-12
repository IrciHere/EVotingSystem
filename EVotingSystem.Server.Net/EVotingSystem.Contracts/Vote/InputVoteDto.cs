namespace EVotingSystem.Contracts.Vote;

public class InputVoteDto
{
    public int ElectionId { get; set; }
    public int CandidateId { get; set; }
    public string VoterPassword { get; set; }
}