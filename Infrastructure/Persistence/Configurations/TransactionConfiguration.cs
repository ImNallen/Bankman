using Domain.Accounts;
using Domain.Shared;
using Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, value => TransactionId.From(value))
            .HasColumnName("id");

        builder.Property(t => t.AccountId)
            .HasConversion(id => id.Value, value => AccountId.From(value))
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasColumnName("type")
            .HasMaxLength(20)
            .IsRequired();

        builder.OwnsOne(t => t.Amount, amount =>
        {
            amount.Property(a => a.Amount)
                .HasColumnName("amount")
                .HasPrecision(18, 4)
                .IsRequired();

            amount.OwnsOne(a => a.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        builder.OwnsOne(t => t.Reference, reference =>
        {
            reference.Property(r => r.Value)
                .HasColumnName("reference")
                .HasMaxLength(TransactionReference.MaxLength);
        });

        builder.Property(t => t.TransferId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? TransferId.From(value.Value) : (TransferId?)null)
            .HasColumnName("transfer_id");

        builder.Property(t => t.OccurredAt)
            .HasColumnName("occurred_at")
            .IsRequired();
    }
}
