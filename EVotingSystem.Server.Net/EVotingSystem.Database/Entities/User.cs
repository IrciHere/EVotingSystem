using System;
using System.Collections.Generic;

namespace EVotingSystem.Database.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool Isadmin { get; set; }

    public virtual ICollection<Electionresult> Electionresults { get; set; } = new List<Electionresult>();

    public virtual ICollection<Electionvote> Electionvotes { get; set; } = new List<Electionvote>();

    public virtual Passwordresetcode? Passwordresetcode { get; set; }

    public virtual Userspassword? Userspassword { get; set; }

    public virtual ICollection<Election> Elections { get; set; } = new List<Election>();

    public virtual ICollection<Election> ElectionsNavigation { get; set; } = new List<Election>();
}
