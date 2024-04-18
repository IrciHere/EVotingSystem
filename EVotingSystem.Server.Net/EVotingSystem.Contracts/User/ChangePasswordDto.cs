namespace EVotingSystem.Contracts.User;

public class ChangePasswordDto
{
    public string OldPassword { get; init; }
    public string NewPassword { get; init; }
}