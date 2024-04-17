namespace EVotingSystem.Contracts.Election;

public class ElectionResultDto
{
    public int CandidateId { get; set; }
    public string CandidateName { get; set; }
    public int Votes { get; set; }
}