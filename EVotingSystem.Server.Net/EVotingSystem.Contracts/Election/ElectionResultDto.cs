namespace EVotingSystem.Contracts.Election;

public class ElectionResultDto
{
    public int CandidateId { get; init; }
    public string CandidateName { get; init; }
    public int Votes { get; init; }
}