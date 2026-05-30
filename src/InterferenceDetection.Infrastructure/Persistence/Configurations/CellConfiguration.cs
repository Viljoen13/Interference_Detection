using InterferenceDetection.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterferenceDetection.Infrastructure.Persistence.Configurations;

public sealed class CellConfiguration : IEntityTypeConfiguration<Cell>
{
    public void Configure(EntityTypeBuilder<Cell> builder)
    {
        builder.ToTable("cells");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(120).IsRequired();
        builder.Property(c => c.CoverageRadiusMeters).IsRequired();
        builder.Property(c => c.TransmitPowerDbm).IsRequired();

        builder.OwnsOne(c => c.Location, loc =>
        {
            loc.Property(l => l.Latitude).HasColumnName("latitude").IsRequired();
            loc.Property(l => l.Longitude).HasColumnName("longitude").IsRequired();
        });

        builder.Ignore(c => c.Technology);

        builder.HasDiscriminator<string>("technology")
            .HasValue<LteCell>("LTE")
            .HasValue<FiveGCell>("5G");
    }
}
