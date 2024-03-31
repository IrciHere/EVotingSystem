namespace EVotingSystem.Database.Entities;

public class UserPassword
{
    public int UserId { get; set; }

    public byte[] PasswordHash { get; set; }

    public virtual User User { get; set; }
}
