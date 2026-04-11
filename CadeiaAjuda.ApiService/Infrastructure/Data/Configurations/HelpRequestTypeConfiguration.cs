using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Configurations;

public class HelpRequestTypeConfiguration : IEntityTypeConfiguration<HelpRequestType>
{
    public void Configure(EntityTypeBuilder<HelpRequestType> builder)
    {
        builder.ToTable("HelpRequestTypes");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Name).IsRequired().HasMaxLength(200);
        builder.Property(h => h.Description).HasMaxLength(500);
        builder.HasIndex(h => new { h.TenantId, h.Name }).IsUnique();

        builder.HasOne(h => h.Tenant).WithMany(t => t.HelpRequestTypes).HasForeignKey(h => h.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.Sector).WithMany().HasForeignKey(h => h.SectorId).OnDelete(DeleteBehavior.Restrict);
    }
}