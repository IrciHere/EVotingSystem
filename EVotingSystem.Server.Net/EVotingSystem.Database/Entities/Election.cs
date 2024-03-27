using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class Election
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime Starttime { get; set; }

    public DateTime Endtime { get; set; }

    public virtual ICollection<Electionresult> Electionresults { get; set; } = new List<Electionresult>();

    public virtual Electionsecret? Electionsecret { get; set; }

    public virtual ICollection<Electionvote> Electionvotes { get; set; } = new List<Electionvote>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<User> UsersNavigation { get; set; } = new List<User>();
}
