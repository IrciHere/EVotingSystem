namespace EVotingSystem.Comparison.Dotnet;

public class VoteEncryptModel
{
    public int CandidateId { get; init; }
    public byte[] VoteHash { get; init; }
}