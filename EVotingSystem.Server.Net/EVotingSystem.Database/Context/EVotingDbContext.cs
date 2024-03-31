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

    public virtual DbSet<UserPassword> UserPasswords { get; set; }

    public virtual DbSet<UserSecret> UserSecrets { get; set; }

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
                .HasColumnName("end_time");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(512)
                .HasColumnName("name");
            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_time");
        });

        modelBuilder.Entity<ElectionResult>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ElectionId }).HasName("candidate_election_result");

            entity.ToTable("election_results");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ElectionId).HasColumnName("election_id");
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
            entity.HasKey(e => e.ElectionId).HasName("election_secrets_pkey");

            entity.ToTable("election_secrets");

            entity.Property(e => e.ElectionId)
                .ValueGeneratedNever()
                .HasColumnName("election_id");
            entity.Property(e => e.Secret)
                .IsRequired()
                .HasColumnName("secret");

            entity.HasOne(d => d.Election).WithOne(p => p.ElectionSecret)
                .HasForeignKey<ElectionSecret>(d => d.ElectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_secret_election");
        });

        modelBuilder.Entity<ElectionVote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("election_votes_pkey");

            entity.ToTable("election_votes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ElectionId).HasColumnName("election_id");
            entity.Property(e => e.IsVerified).HasColumnName("is_verified");
            entity.Property(e => e.VoteHash)
                .IsRequired()
                .HasColumnName("vote_hash");
            entity.Property(e => e.VotedCandidateEncrypted)
                .IsRequired()
                .HasColumnName("voted_candidate_encrypted");
            entity.Property(e => e.VotedCandidateId).HasColumnName("voted_candidate_id");

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
            entity.HasKey(e => e.UserId).HasName("password_reset_codes_pkey");

            entity.ToTable("password_reset_codes");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.ResetCode)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("reset_code");

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
                .IsRequired()
                .HasMaxLength(320)
                .HasColumnName("email");
            entity.Property(e => e.IsAdmin).HasColumnName("is_admin");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15)
                .HasColumnName("phone_number");

            entity.HasMany(d => d.Elections).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "ElectionCandidate",
                    r => r.HasOne<Election>().WithMany()
                        .HasForeignKey("ElectionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_candidate_election"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_candidate_user"),
                    j =>
                    {
                        j.HasKey("UserId", "ElectionId").HasName("candidate_election");
                        j.ToTable("election_candidates");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("ElectionId").HasColumnName("election_id");
                    });

            entity.HasMany(d => d.ElectionsNavigation).WithMany(p => p.UsersNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "EligibleVoter",
                    r => r.HasOne<Election>().WithMany()
                        .HasForeignKey("ElectionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_voter_election"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_voter_user"),
                    j =>
                    {
                        j.HasKey("UserId", "ElectionId").HasName("voter_election");
                        j.ToTable("eligible_voters");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("ElectionId").HasColumnName("election_id");
                    });
        });

        modelBuilder.Entity<UserPassword>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_passwords_pkey");

            entity.ToTable("user_passwords");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasColumnName("password_hash");

            entity.HasOne(d => d.User).WithOne(p => p.UserPassword)
                .HasForeignKey<UserPassword>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_password_user");
        });

        modelBuilder.Entity<UserSecret>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_secrets_pkey");

            entity.ToTable("user_secrets");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.PasswordSalt)
                .IsRequired()
                .HasColumnName("password_salt");
            entity.Property(e => e.VotingSecret)
                .IsRequired()
                .HasColumnName("voting_secret");

            entity.HasOne(d => d.User).WithOne(p => p.UserSecret)
                .HasForeignKey<UserSecret>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_secret_user");
        });

        modelBuilder.Entity<VotesOtp>(entity =>
        {
            entity.HasKey(e => e.VoteId).HasName("votes_otp_pkey");

            entity.ToTable("votes_otp");

            entity.Property(e => e.VoteId)
                .ValueGeneratedOnAdd()
                .HasColumnName("vote_id");
            entity.Property(e => e.OtpCode)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("otp_code");

            entity.HasOne(d => d.Vote).WithOne(p => p.VotesOtp)
                .HasForeignKey<VotesOtp>(d => d.VoteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_otp_vote");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
