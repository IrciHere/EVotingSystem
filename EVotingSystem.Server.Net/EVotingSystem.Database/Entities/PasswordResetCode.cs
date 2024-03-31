﻿using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class PasswordResetCode
{
    public int UserId { get; set; }

    public string ResetCode { get; set; }

    public virtual User User { get; set; }
}
