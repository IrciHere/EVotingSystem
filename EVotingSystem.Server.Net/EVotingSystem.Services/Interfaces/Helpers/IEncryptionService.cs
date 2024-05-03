namespace EVotingSystem.Services.Interfaces.Helpers;

public interface IEncryptionService
{
    byte[] HashSHA256(string password);
    byte[] HashSHA256WithSalt(string password, byte[] salt);
    byte[] EncryptVote(int candidateId, int electionId, byte[] electionSecret, byte[] voteHash);
    byte[] EncryptVotingSecretForUser(string secret, string password, int userId);
    string DecryptVotingSecretForUser(byte[] secret, string password, int userId);
    string DecryptSecret(byte[] encryptedSecret, byte[] key, byte[] iv);
    byte[] GenerateRandomByteArray();
    byte[] GenerateIVArrayFromId(int id);
}