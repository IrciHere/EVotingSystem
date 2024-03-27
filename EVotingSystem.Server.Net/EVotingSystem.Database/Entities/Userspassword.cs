using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class Userspassword
{
    public int Userid { get; set; }

    public byte[] Passwordhash { get; set; } = null!;

    public byte[] Passwordsalt { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
