using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class VotesOtp
{
    public int VoteId { get; set; }

    public string OtpCode { get; set; }

    public virtual ElectionVote Vote { get; set; }
}
