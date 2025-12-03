using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxChecker.Domain;

namespace TaxChecker.Infrastructure.Data.Configurations;

public class TaxRuleConfiguration : IEntityTypeConfiguration<TaxRule>
{
    public void Configure(EntityTypeBuilder<TaxRule> builder)
    {
        builder.ToTable("TaxRules");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.Rate)
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        builder.Property(t => t.ValidFrom).IsRequired();
        builder.Property(t => t.ValidTo).IsRequired();

        // Relations
        builder.HasOne<City>()
            .WithMany()
            .HasForeignKey(t => t.CityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
