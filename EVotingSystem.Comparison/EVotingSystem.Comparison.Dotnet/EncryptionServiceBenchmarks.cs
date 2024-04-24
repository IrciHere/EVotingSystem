using BenchmarkDotNet.Attributes;

namespace EVotingSystem.Comparison.Dotnet;

[MemoryDiagnoser]
public class EncryptionServiceBenchmarks
{
    private const string Password = "TestPassw0rd%";
    private const string Secret = "TestSecr3t@";
    private byte[] _aesKey;
    private byte[] _aesIv;

    [GlobalSetup]
    public void Setup()
    {
        _aesKey = Enumerable.Range(0, 32)
            .Select(num => (byte)(num * 3))
            .ToArray();

        _aesIv = Enumerable.Range(1, 16)
            .Select(num => (byte)(num * 4 + 7))
            .ToArray();
    }

    [Benchmark]
    public byte[] CreateHash()
    {
        byte[] hash = EncryptionService.HashSHA256(Password);

        return hash;
    }

    [Benchmark]
    public byte[] EncryptSecret()
    {
        byte[] encrypted = EncryptionService.EncryptSecret(Secret, _aesKey, _aesIv);

        return encrypted;
    }

    [Benchmark]
    public string EncryptAndDecryptSecret()
    {
        byte[] encrypted = EncryptionService.EncryptSecret(Secret, _aesKey, _aesIv);
        string decrypted = EncryptionService.DecryptSecret(encrypted, _aesKey, _aesIv);
        
        return decrypted;
    }
}