﻿namespace EVotingSystem.Services.Interfaces;

public interface IEmailsService
{
    Task SendResetPasswordMail(string email, string code);
}