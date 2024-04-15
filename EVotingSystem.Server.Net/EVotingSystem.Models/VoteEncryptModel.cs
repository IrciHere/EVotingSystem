namespace EVotingSystem.Models;

public class VoteEncryptModel
{
    public int CandidateId { get; set; }
    public byte[] VoteHash { get; set; }
}