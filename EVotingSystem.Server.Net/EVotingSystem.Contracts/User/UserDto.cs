namespace EVotingSystem.Contracts.User;

public class UserDto
{
    public int Id { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
    public string Name { get; init; }
    public bool IsAdmin { get; init; }
}