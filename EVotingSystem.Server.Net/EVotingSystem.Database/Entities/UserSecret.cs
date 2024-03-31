using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class UserSecret
{
    public int UserId { get; set; }

    public byte[] PasswordSalt { get; set; }

    public byte[] VotingSecret { get; set; }

    public virtual User User { get; set; }
}
