using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class ElectionVote
{
    public int Id { get; set; }

    public int? VotedCandidateId { get; set; }

    public int ElectionId { get; set; }

    public byte[] VoteHash { get; set; }

    public byte[] VotedCandidateEncrypted { get; set; }

    public bool IsVerified { get; set; }

    public virtual Election Election { get; set; }

    public virtual User VotedCandidate { get; set; }

    public virtual VotesOtp VotesOtp { get; set; }
}
