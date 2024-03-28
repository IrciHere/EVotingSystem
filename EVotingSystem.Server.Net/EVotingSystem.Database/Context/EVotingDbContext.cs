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

    public virtual DbSet<ElectionResult> ElectionResults { get; set; }

    public virtual DbSet<ElectionSecret> ElectionSecrets { get; set; }

    public virtual DbSet<ElectionVote> ElectionVotes { get; set; }

    public virtual DbSet<PasswordResetCode> PasswordResetCodes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersPassword> UsersPasswords { get; set; }

    public virtual DbSet<VotesOtp> VotesOtps { get; set; }

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
            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("endtime");
            entity.Property(e => e.Name)
                .HasMaxLength(512)
                .HasColumnName("name");
            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("starttime");
        });

        modelBuilder.Entity<ElectionResult>(entity =>
        {
            entity.HasKey(e => new { Userid = e.UserId, Electionid = e.ElectionId }).HasName("candidate_election_result");

            entity.ToTable("electionresults");

            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.ElectionId).HasColumnName("electionid");
            entity.Property(e => e.Votes).HasColumnName("votes");

            entity.HasOne(d => d.Election).WithMany(p => p.ElectionResults)
                .HasForeignKey(d => d.ElectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_result_election");

            entity.HasOne(d => d.User).WithMany(p => p.ElectionResults)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_result_user");
        });

        modelBuilder.Entity<ElectionSecret>(entity =>
        {
            entity.HasKey(e => e.ElectionId).HasName("electionsecrets_pkey");

            entity.ToTable("electionsecrets");

            entity.Property(e => e.ElectionId)
                .ValueGeneratedNever()
                .HasColumnName("electionid");
            entity.Property(e => e.Secret).HasColumnName("secret");

            entity.HasOne(d => d.Election).WithOne(p => p.ElectionSecret)
                .HasForeignKey<ElectionSecret>(d => d.ElectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_secret_election");
        });

        modelBuilder.Entity<ElectionVote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("electionvotes_pkey");

            entity.ToTable("electionvotes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ElectionId).HasColumnName("electionid");
            entity.Property(e => e.IsVerified).HasColumnName("isverified");
            entity.Property(e => e.VotedCandidateEncrypted).HasColumnName("votedcandidateencrypted");
            entity.Property(e => e.VotedCandidateId).HasColumnName("votedcandidateid");
            entity.Property(e => e.VoteHash).HasColumnName("votehash");

            entity.HasOne(d => d.Election).WithMany(p => p.ElectionVotes)
                .HasForeignKey(d => d.ElectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vote_election");

            entity.HasOne(d => d.VotedCandidate).WithMany(p => p.ElectionVotes)
                .HasForeignKey(d => d.VotedCandidateId)
                .HasConstraintName("fk_vote_candidate");
        });

        modelBuilder.Entity<PasswordResetCode>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("passwordresetcodes_pkey");

            entity.ToTable("passwordresetcodes");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("userid");
            entity.Property(e => e.ResetCode)
                .HasMaxLength(10)
                .HasColumnName("resetcode");

            entity.HasOne(d => d.User).WithOne(p => p.PasswordResetCode)
                .HasForeignKey<PasswordResetCode>(d => d.UserId)
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
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phonenumber");
            entity.Property(e => e.IsAdmin).HasColumnName("isadmin");
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

        modelBuilder.Entity<UsersPassword>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("userspasswords_pkey");

            entity.ToTable("userspasswords");

            entity.Property(e => e.Userid)
                .ValueGeneratedNever()
                .HasColumnName("userid");
            entity.Property(e => e.PasswordHash).HasColumnName("passwordhash");
            entity.Property(e => e.PasswordSalt).HasColumnName("passwordsalt");

            entity.HasOne(d => d.User).WithOne(p => p.UsersPassword)
                .HasForeignKey<UsersPassword>(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_password_user");
        });

        modelBuilder.Entity<VotesOtp>(entity =>
        {
            entity.HasKey(e => e.VoteId).HasName("votesotp_pkey");

            entity.ToTable("votesotp");

            entity.Property(e => e.VoteId)
                .ValueGeneratedOnAdd()
                .HasColumnName("voteid");
            entity.Property(e => e.OtpCode)
                .HasMaxLength(10)
                .HasColumnName("otpcode");

            entity.HasOne(d => d.Vote).WithOne(p => p.VotesOtp)
                .HasForeignKey<VotesOtp>(d => d.VoteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_otp_vote");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
