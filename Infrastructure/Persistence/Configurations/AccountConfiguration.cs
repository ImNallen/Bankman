using Domain.Accounts;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, value => AccountId.From(value))
            .HasColumnName("id");

        builder.Property(a => a.OwnerId)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .HasColumnName("owner_id")
            .IsRequired();

        builder.OwnsOne(a => a.Number, number =>
        {
            number.Property(n => n.Value)
                .HasColumnName("account_number")
                .HasMaxLength(34)
                .IsRequired();

            number.HasIndex(n => n.Value).IsUnique();
        });

        builder.OwnsOne(a => a.Balance, balance =>
        {
            balance.Property(b => b.Amount)
                .HasColumnName("balance_amount")
                .HasPrecision(18, 4)
                .IsRequired();

            balance.OwnsOne(b => b.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("balance_currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.OpenedAt)
            .HasColumnName("opened_at")
            .IsRequired();

        builder.Property(a => a.ClosedAt)
            .HasColumnName("closed_at");
    }
}
