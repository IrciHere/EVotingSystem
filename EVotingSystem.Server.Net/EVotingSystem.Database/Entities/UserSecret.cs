namespace EVotingSystem.Database.Entities;

public class UserSecret
{
    public int UserId { get; set; }

    public byte[] PasswordSalt { get; set; }

    public byte[] VotingSecret { get; set; }

    public virtual User User { get; set; }
}
