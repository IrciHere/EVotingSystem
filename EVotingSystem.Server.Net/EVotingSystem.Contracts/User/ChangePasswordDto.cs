﻿namespace EVotingSystem.Contracts.User;

public class ChangePasswordDto
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}