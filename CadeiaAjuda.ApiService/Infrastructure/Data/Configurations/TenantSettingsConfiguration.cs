using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Configurations;

public class TenantSettingsConfiguration : IEntityTypeConfiguration<TenantSettings>
{
    public void Configure(EntityTypeBuilder<TenantSettings> builder)
    {
        builder.ToTable("TenantSettings");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.DisplayName).IsRequired().HasMaxLength(200);
        builder.Property(s => s.LogoUrl).HasMaxLength(500);
        builder.Property(s => s.PrimaryColor).IsRequired().HasMaxLength(10);
        builder.Property(s => s.TimeZone).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Language).IsRequired().HasMaxLength(10);
        builder.HasIndex(s => s.TenantId).IsUnique();

        builder.HasOne(s => s.Tenant).WithOne(t => t.Settings).HasForeignKey<TenantSettings>(s => s.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}