namespace EVotingSystem.Contracts.User;

public class BaseUserDto
{
    public string Email { get; set; }
    
    public string PhoneNumber { get; set; }

    public string Name { get; set; }

    public bool IsAdmin { get; set; }
}