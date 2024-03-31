using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class ElectionResult
{
    public int UserId { get; set; }

    public int ElectionId { get; set; }

    public int Votes { get; set; }

    public virtual Election Election { get; set; }

    public virtual User User { get; set; }
}
