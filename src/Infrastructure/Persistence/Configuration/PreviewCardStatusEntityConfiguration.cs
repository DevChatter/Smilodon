using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class PreviewCardStatusEntityConfiguration : IEntityTypeConfiguration<PreviewCardStatus>
{
    public void Configure(EntityTypeBuilder<PreviewCardStatus> builder)
    {
        builder.HasNoKey();

        builder.ToTable("preview_cards_statuses");

        builder.HasIndex(e => new { e.StatusId, e.PreviewCardId })
            .HasDatabaseName("index_preview_cards_statuses_on_status_id_and_preview_card_id");

        builder.Property(e => e.PreviewCardId).HasColumnName("preview_card_id");

        builder.Property(e => e.StatusId).HasColumnName("status_id");
    }
}