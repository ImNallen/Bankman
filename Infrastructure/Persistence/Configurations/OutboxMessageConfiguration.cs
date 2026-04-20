using Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");
        builder.Property(m => m.Type).HasColumnName("type").HasMaxLength(500).IsRequired();
        builder.Property(m => m.Content).HasColumnName("content").IsRequired();
        builder.Property(m => m.OccurredOn).HasColumnName("occurred_on").IsRequired();
        builder.Property(m => m.ProcessedOn).HasColumnName("processed_on");
        builder.Property(m => m.Error).HasColumnName("error").HasMaxLength(2000);
        builder.HasIndex(m => m.ProcessedOn).HasDatabaseName("ix_outbox_messages_processed_on");
    }
}
