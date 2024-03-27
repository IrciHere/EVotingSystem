using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class Electionsecret
{
    public int Electionid { get; set; }

    public byte[] Secret { get; set; } = null!;

    public virtual Election Election { get; set; } = null!;
}
