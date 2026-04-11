using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Configurations;

public class PriorityConfiguration : IEntityTypeConfiguration<Priority>
{
    public void Configure(EntityTypeBuilder<Priority> builder)
    {
        builder.ToTable("Priorities");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Color).IsRequired().HasMaxLength(10);
        builder.HasIndex(p => new { p.TenantId, p.Name }).IsUnique();

        builder.HasOne(p => p.Tenant).WithMany(t => t.Priorities).HasForeignKey(p => p.TenantId).OnDelete(DeleteBehavior.Restrict);
    }
}