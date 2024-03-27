using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class Votesotp
{
    public int Voteid { get; set; }

    public string Otpcode { get; set; } = null!;

    public virtual Electionvote Vote { get; set; } = null!;
}
