namespace EVotingSystem.Database.Entities;

public class ElectionSecret
{
    public int ElectionId { get; set; }

    public byte[] Secret { get; set; }

    public virtual Election Election { get; set; }
}
