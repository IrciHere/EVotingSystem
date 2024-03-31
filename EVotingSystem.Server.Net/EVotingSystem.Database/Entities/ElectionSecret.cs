using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class ElectionSecret
{
    public int ElectionId { get; set; }

    public byte[] Secret { get; set; }

    public virtual Election Election { get; set; }
}
