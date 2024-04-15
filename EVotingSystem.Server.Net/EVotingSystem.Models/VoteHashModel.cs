namespace EVotingSystem.Models;

public class VoteHashModel
{
    public int UserId { get; set; }
    public int ElectionId { get; set; }
    public string Secret { get; set; }
}