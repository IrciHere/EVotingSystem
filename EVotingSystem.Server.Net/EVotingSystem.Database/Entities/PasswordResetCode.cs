namespace EVotingSystem.Database.Entities;

public class PasswordResetCode
{
    public int UserId { get; set; }

    public string ResetCode { get; set; }

    public virtual User User { get; set; }
}
