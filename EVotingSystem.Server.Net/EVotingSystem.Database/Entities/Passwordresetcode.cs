using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class Passwordresetcode
{
    public int Userid { get; set; }

    public string Resetcode { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
