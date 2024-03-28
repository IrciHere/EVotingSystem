using System;
using System.Collections.Generic;
using EVotingSystem.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EVotingSystem.Database.Context;

public partial class EVotingDbContext : DbContext
{
    public EVotingDbContext()
    {
    }

    public EVotingDbContext(DbContextOptions<EVotingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Election> Elections { get; set; }

    public virtual DbSet<Electionresult> Electionresults { get; set; }

    public virtual DbSet<Electionsecret> Electionsecrets { get; set; }

    public virtual DbSet<Electionvote> Electionvotes { get; set; }

    public virtual DbSet<Passwordresetcode> Passwordresetcodes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userspassword> Userspasswords { get; set; }

    public virtual DbSet<Votesotp> Votesotps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID=postgres;Password=zaq1@WSX;Host=localhost;Port=5432;Database=evotingsystem;Pooling=true;MinPoolSize=0;MaxPoolSize=100;Connection Lifetime=0;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Election>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("elections_pkey");

            entity.ToTable("elections");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Endtime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("endtime");
            entity.Property(e => e.Name)
                .HasMaxLength(512)
                .HasColumnName("name");
            entity.Property(e => e.Starttime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("starttime");
        });

        modelBuilder.Entity<Electionresult>(entity =>
        {
            entity.HasKey(e => new { e.Userid, e.Electionid }).HasName("candidate_election_result");

            entity.ToTable("electionresults");

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Electionid).HasColumnName("electionid");
            entity.Property(e => e.Votes).HasColumnName("votes");

            entity.HasOne(d => d.Election).WithMany(p => p.Electionresults)
                .HasForeignKey(d => d.Electionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_result_election");

            entity.HasOne(d => d.User).WithMany(p => p.Electionresults)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_result_user");
        });

        modelBuilder.Entity<Electionsecret>(entity =>
        {
            entity.HasKey(e => e.Electionid).HasName("electionsecrets_pkey");

            entity.ToTable("electionsecrets");

            entity.Property(e => e.Electionid)
                .ValueGeneratedNever()
                .HasColumnName("electionid");
            entity.Property(e => e.Secret).HasColumnName("secret");

            entity.HasOne(d => d.Election).WithOne(p => p.Electionsecret)
                .HasForeignKey<Electionsecret>(d => d.Electionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_secret_election");
        });

        modelBuilder.Entity<Electionvote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("electionvotes_pkey");

            entity.ToTable("electionvotes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Electionid).HasColumnName("electionid");
            entity.Property(e => e.Isverified).HasColumnName("isverified");
            entity.Property(e => e.Votedcandidateencrypted).HasColumnName("votedcandidateencrypted");
            entity.Property(e => e.Votedcandidateid).HasColumnName("votedcandidateid");
            entity.Property(e => e.Votehash).HasColumnName("votehash");

            entity.HasOne(d => d.Election).WithMany(p => p.Electionvotes)
                .HasForeignKey(d => d.Electionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vote_election");

            entity.HasOne(d => d.Votedcandidate).WithMany(p => p.Electionvotes)
                .HasForeignKey(d => d.Votedcandidateid)
                .HasConstraintName("fk_vote_candidate");
        });

        modelBuilder.Entity<Passwordresetcode>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("passwordresetcodes_pkey");

            entity.ToTable("passwordresetcodes");

            entity.Property(e => e.Userid)
                .ValueGeneratedNever()
                .HasColumnName("userid");
            entity.Property(e => e.Resetcode)
                .HasMaxLength(10)
                .HasColumnName("resetcode");

            entity.HasOne(d => d.User).WithOne(p => p.Passwordresetcode)
                .HasForeignKey<Passwordresetcode>(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_resetcode_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(320)
                .HasColumnName("email");
            entity.Property(e => e.Isadmin).HasColumnName("isadmin");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasMany(d => d.Elections).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "Electioncandidate",
                    r => r.HasOne<Election>().WithMany()
                        .HasForeignKey("Electionid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_candidate_election"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Userid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_candidate_user"),
                    j =>
                    {
                        j.HasKey("Userid", "Electionid").HasName("candidate_election");
                        j.ToTable("electioncandidates");
                        j.IndexerProperty<int>("Userid").HasColumnName("userid");
                        j.IndexerProperty<int>("Electionid").HasColumnName("electionid");
                    });

            entity.HasMany(d => d.ElectionsNavigation).WithMany(p => p.UsersNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "Eligiblevoter",
                    r => r.HasOne<Election>().WithMany()
                        .HasForeignKey("Electionid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_voter_election"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Userid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_voter_user"),
                    j =>
                    {
                        j.HasKey("Userid", "Electionid").HasName("voter_election");
                        j.ToTable("eligiblevoters");
                        j.IndexerProperty<int>("Userid").HasColumnName("userid");
                        j.IndexerProperty<int>("Electionid").HasColumnName("electionid");
                    });
        });

        modelBuilder.Entity<Userspassword>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("userspasswords_pkey");

            entity.ToTable("userspasswords");

            entity.Property(e => e.Userid)
                .ValueGeneratedNever()
                .HasColumnName("userid");
            entity.Property(e => e.Passwordhash).HasColumnName("passwordhash");
            entity.Property(e => e.Passwordsalt).HasColumnName("passwordsalt");

            entity.HasOne(d => d.User).WithOne(p => p.Userspassword)
                .HasForeignKey<Userspassword>(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_password_user");
        });

        modelBuilder.Entity<Votesotp>(entity =>
        {
            entity.HasKey(e => e.Voteid).HasName("votesotp_pkey");

            entity.ToTable("votesotp");

            entity.Property(e => e.Voteid)
                .ValueGeneratedOnAdd()
                .HasColumnName("voteid");
            entity.Property(e => e.Otpcode)
                .HasMaxLength(10)
                .HasColumnName("otpcode");

            entity.HasOne(d => d.Vote).WithOne(p => p.Votesotp)
                .HasForeignKey<Votesotp>(d => d.Voteid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_otp_vote");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
