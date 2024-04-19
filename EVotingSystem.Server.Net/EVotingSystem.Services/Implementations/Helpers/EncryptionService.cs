﻿using System.Security.Cryptography;
using System.Text;
using EVotingSystem.Services.Interfaces.Helpers;

namespace EVotingSystem.Services.Implementations.Helpers;

public class EncryptionService : IEncryptionService
{
    public byte[] HashSHA256(string password)
    {
        byte[] passwordAsByteArray = Encoding.UTF8
            .GetBytes(password)
            .ToArray();

        byte[] hashedPassword = SHA256.HashData(passwordAsByteArray);

        return hashedPassword;
    }

    public byte[] HashSHA256WithSalt(string password, byte[] salt)
    {
        byte[] saltedPassword = Encoding.UTF8
            .GetBytes(password)
            .Concat(salt)
            .ToArray();

        byte[] hashedPassword = SHA256.HashData(saltedPassword);

        return hashedPassword;
    }

    public byte[] EncryptVotingSecretForUser(string secret, string password, int userId)
    {
        // SHA256 hash of password without salt is used as AES encryption key
        byte[] votingSecretEncryptionKey = HashSHA256(password);
        byte[] votingSecretEncryptionIV = GenerateIVArrayFromUserId(userId);
        byte[] votingSecretEncrypted = EncryptSecret(secret, votingSecretEncryptionKey, votingSecretEncryptionIV);

        return votingSecretEncrypted;
    }

    public string DecryptVotingSecretForUser(byte[] secret, string password, int userId)
    {
        // SHA256 hash of password without salt is used as AES encryption key
        byte[] oldVotingSecretEncryptionKey = HashSHA256(password);
        byte[] votingSecretEncryptionIV = GenerateIVArrayFromUserId(userId);
        string votingSecretDecrypted = DecryptSecret(secret, oldVotingSecretEncryptionKey, votingSecretEncryptionIV);

        return votingSecretDecrypted;
    }

    public byte[] EncryptSecret(string secret, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        ICryptoTransform encryptor = aes.CreateEncryptor();

        // Create the streams used for encryption.
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            //Write all data to the stream.
            swEncrypt.Write(secret);
        }
        
        byte[] encryptedSecret = msEncrypt.ToArray();

        return encryptedSecret;
    }

    public string DecryptSecret(byte[] encryptedSecret, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        
        ICryptoTransform decryptor = aes.CreateDecryptor();

        using var msDecrypt = new MemoryStream(encryptedSecret);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        string secret = srDecrypt.ReadToEnd();

        return secret;
    }

    public byte[] GenerateRandomByteArray()
    {
        var rng = RandomNumberGenerator.Create();
        var salt = new byte[32];
             
        rng.GetBytes(salt);

        return salt;
    }

    public byte[] GenerateIVArrayFromUserId(int userId)
    {
        var random = new Random(userId);

        var ivArray = new byte[16];
        random.NextBytes(ivArray);

        return ivArray;
    }
}