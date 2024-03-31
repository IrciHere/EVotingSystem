using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class UserPassword
{
    public int UserId { get; set; }

    public byte[] PasswordHash { get; set; }

    public virtual User User { get; set; }
}
