using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class ListAccountEntityConfiguration : IEntityTypeConfiguration<ListAccount>
{
    public void Configure(EntityTypeBuilder<ListAccount> builder)
    {
        builder.ToTable("list_accounts");
        
        builder.HasKey(e => e.Id).HasName("list_accounts_pkey");

        builder.HasIndex(e => new { e.AccountId, e.ListId })
            .HasDatabaseName("index_list_accounts_on_account_id_and_list_id")
            .IsUnique();

        builder.HasIndex(e => e.FollowId)
            .HasDatabaseName("index_list_accounts_on_follow_id")
            .HasFilter("(follow_id IS NOT NULL)");

        builder.HasIndex(e => new { e.ListId, e.AccountId })
            .HasDatabaseName("index_list_accounts_on_list_id_and_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.FollowId).HasColumnName("follow_id");

        builder.Property(e => e.ListId).HasColumnName("list_id");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.ListAccounts)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_rails_85fee9d6ab");

        builder.HasOne(d => d.Follow)
            .WithMany(p => p.ListAccounts)
            .HasForeignKey(d => d.FollowId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_40f9cc29f1");

        builder.HasOne(d => d.List)
            .WithMany(p => p.ListAccounts)
            .HasForeignKey(d => d.ListId)
            .HasConstraintName("fk_rails_e54e356c88");
    }
}