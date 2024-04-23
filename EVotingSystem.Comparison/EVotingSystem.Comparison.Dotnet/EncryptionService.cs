namespace EVotingSystem.Comparison.Dotnet;

using System.Security.Cryptography;
using System.Text;

public static class EncryptionService
{
    public static byte[] HashSHA256(string password)
    {
        byte[] passwordAsByteArray = Encoding.UTF8
            .GetBytes(password)
            .ToArray();

        byte[] hashedPassword = SHA256.HashData(passwordAsByteArray);

        return hashedPassword;
    }
    
    public static byte[] EncryptSecret(string secret, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        ICryptoTransform encryptor = aes.CreateEncryptor();

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(secret);
        }
        
        byte[] encryptedSecret = msEncrypt.ToArray();

        return encryptedSecret;
    }

    public static string DecryptSecret(byte[] encryptedSecret, byte[] key, byte[] iv)
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
}