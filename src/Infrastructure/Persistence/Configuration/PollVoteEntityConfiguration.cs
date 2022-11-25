using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class PollVoteEntityConfiguration : IEntityTypeConfiguration<PollVote>
{
    public void Configure(EntityTypeBuilder<PollVote> builder)
    {
        builder.ToTable("poll_votes");
        
        builder.HasKey(e => e.Id).HasName("poll_votes_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_poll_votes_on_account_id");

        builder.HasIndex(e => e.PollId).HasDatabaseName("index_poll_votes_on_poll_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Choice).HasColumnName("choice");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.PollId).HasColumnName("poll_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Uri)
            .HasColumnType("character varying")
            .HasColumnName("uri");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.PollVotes)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_b6c18cf44a");

        builder.HasOne(d => d.Poll)
            .WithMany(p => p.PollVotes)
            .HasForeignKey(d => d.PollId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_a6e6974b7e");
    }
}