namespace EVotingSystem.Contracts.Election;

public class ElectionDto
{
    public int Id { get; init; }
    public bool HasEnded { get; init; }
    public string Name { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; } 
}