﻿namespace EVotingSystem.Services.Interfaces.Helpers;

public interface IEncryptionService
{
    byte[] HashSHA256(string password);
    byte[] HashSHA256WithSalt(string password, byte[] salt);
    byte[] EncryptVotingSecretForUser(string secret, string password, int userId);
    string DecryptVotingSecretForUser(byte[] secret, string password, int userId);
    byte[] EncryptSecret(string secret, byte[] key, byte[] iv);
    string DecryptSecret(byte[] encryptedSecret, byte[] key, byte[] iv);
    byte[] GenerateRandomByteArray();
    byte[] GenerateIVArrayFromUserId(int userId);
}