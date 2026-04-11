using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Configurations;

public class SectorConfiguration : IEntityTypeConfiguration<Sector>
{
    public void Configure(EntityTypeBuilder<Sector> builder)
    {
        builder.ToTable("Sectors");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.Property(s => s.Color).IsRequired().HasMaxLength(10);
        builder.HasIndex(s => new { s.TenantId, s.Name }).IsUnique();

        builder.HasOne(s => s.Tenant).WithMany(t => t.Sectors).HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Restrict);
    }
}