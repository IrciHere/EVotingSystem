namespace EVotingSystem.Models;

public class VoteHashModel
{
    public int UserId { get; init; }
    public int ElectionId { get; init; }
    public string Secret { get; init; }
}