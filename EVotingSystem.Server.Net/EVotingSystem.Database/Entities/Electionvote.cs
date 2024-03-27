using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class Electionvote
{
    public int Id { get; set; }

    public int? Votedcandidateid { get; set; }

    public int Electionid { get; set; }

    public byte[] Votehash { get; set; } = null!;

    public byte[] Votedcandidateencrypted { get; set; } = null!;

    public bool Isverified { get; set; }

    public virtual Election Election { get; set; } = null!;

    public virtual User? Votedcandidate { get; set; }

    public virtual Votesotp? Votesotp { get; set; }
}
