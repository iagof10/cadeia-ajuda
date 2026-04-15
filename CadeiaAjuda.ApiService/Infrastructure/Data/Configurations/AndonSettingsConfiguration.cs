using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Configurations;

public class AndonSettingsConfiguration : IEntityTypeConfiguration<AndonSettings>
{
    public void Configure(EntityTypeBuilder<AndonSettings> builder)
    {
        builder.ToTable("AndonSettings");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.TenantId).IsUnique();

        builder.HasOne(s => s.Tenant).WithMany().HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}
