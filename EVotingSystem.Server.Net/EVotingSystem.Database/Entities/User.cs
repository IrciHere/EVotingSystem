using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string Name { get; set; }

    public bool IsAdmin { get; set; }

    public virtual ICollection<ElectionResult> ElectionResults { get; set; } = new List<ElectionResult>();

    public virtual ICollection<ElectionVote> ElectionVotes { get; set; } = new List<ElectionVote>();

    public virtual PasswordResetCode PasswordResetCode { get; set; }

    public virtual UserPassword UserPassword { get; set; }

    public virtual UserSecret UserSecret { get; set; }

    public virtual ICollection<Election> Elections { get; set; } = new List<Election>();

    public virtual ICollection<Election> ElectionsNavigation { get; set; } = new List<Election>();
}
