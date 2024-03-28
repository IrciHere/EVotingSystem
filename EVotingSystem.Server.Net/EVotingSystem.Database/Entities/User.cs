namespace EVotingSystem.Database.Entities;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; }
    
    public string PhoneNumber { get; set; }

    public string Name { get; set; }

    public bool IsAdmin { get; set; }

    public virtual ICollection<ElectionResult> ElectionResults { get; set; }

    public virtual ICollection<ElectionVote> ElectionVotes { get; set; }

    public virtual PasswordResetCode PasswordResetCode { get; set; }

    public virtual UsersPassword UsersPassword { get; set; }

    public virtual ICollection<Election> Elections { get; set; }

    public virtual ICollection<Election> ElectionsNavigation { get; set; }
}
