namespace EVotingSystem.Contracts.Election;

public class ElectionDto : BaseElectionDto
{
    public int Id { get; set; }
    public bool HasFinished { get; set; }
}