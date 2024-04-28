namespace EVotingSystem.Contracts.Election;

public class NewElectionDto
{
    public string Name { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; } 
}