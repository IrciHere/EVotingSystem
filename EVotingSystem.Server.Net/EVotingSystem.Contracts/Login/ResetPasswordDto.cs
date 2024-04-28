namespace EVotingSystem.Contracts.Login;

public class ResetPasswordDto
{
    public string ResetCode { get; init; }
    public string NewPassword { get; init; }
}