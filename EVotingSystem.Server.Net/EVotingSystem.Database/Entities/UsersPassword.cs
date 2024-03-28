namespace EVotingSystem.Database.Entities;

public class UsersPassword
{
    public int Userid { get; set; }

    public byte[] PasswordHash { get; set; }

    public byte[] PasswordSalt { get; set; }

    public virtual User User { get; set; }
}
