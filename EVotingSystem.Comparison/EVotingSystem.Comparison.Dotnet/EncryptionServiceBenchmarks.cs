using BenchmarkDotNet.Attributes;

namespace EVotingSystem.Comparison.Dotnet;

public class EncryptionServiceBenchmarks
{
    private string _textForTests;
    private byte[] _aesKey;
    private byte[] _aesIv;
    
    [Params(100, 500, 1000, 5000, 10_000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _aesKey = Enumerable.Range(0, 32)
            .Select(num => (byte)(num * 3))
            .ToArray();

        _aesIv = Enumerable.Range(1, 16)
            .Select(num => (byte)(num * 4 + 7))
            .ToArray();

        _textForTests = Secret.Text[..Size];
    }

    [Benchmark]
    public byte[] CreateHash()
    {
        byte[] hash = EncryptionService.HashSHA256(_textForTests);

        return hash;
    }

    [Benchmark]
    public byte[] EncryptSecret()
    {
        byte[] encrypted = EncryptionService.EncryptSecret(_textForTests, _aesKey, _aesIv);

        return encrypted;
    }

    [Benchmark]
    public string EncryptAndDecryptSecret()
    {
        byte[] encrypted = EncryptionService.EncryptSecret(_textForTests, _aesKey, _aesIv);
        string decrypted = EncryptionService.DecryptSecret(encrypted, _aesKey, _aesIv);
        
        return decrypted;
    }
}