namespace EVotingSystem.Contracts.Vote;

public class HashCheckDto
{
    public int ElectionId { get; init; }
    public string Password { get; init; }
}