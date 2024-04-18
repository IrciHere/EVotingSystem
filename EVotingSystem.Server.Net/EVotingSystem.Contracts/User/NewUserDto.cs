namespace EVotingSystem.Contracts.User;

public class NewUserDto
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Name { get; set; }
    public bool IsAdmin { get; set; }
}