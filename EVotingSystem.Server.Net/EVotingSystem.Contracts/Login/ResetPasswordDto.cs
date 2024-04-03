namespace EVotingSystem.Contracts.Login;

public class ResetPasswordDto
{
    public string ResetCode { get; set; }
    public string NewPassword { get; set; }
}