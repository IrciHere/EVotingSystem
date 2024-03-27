using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class Electionresult
{
    public int Userid { get; set; }

    public int Electionid { get; set; }

    public int Votes { get; set; }

    public virtual Election Election { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
