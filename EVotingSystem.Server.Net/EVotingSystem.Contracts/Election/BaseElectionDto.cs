namespace EVotingSystem.Contracts.Election;

public class BaseElectionDto
{
    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; } 
}