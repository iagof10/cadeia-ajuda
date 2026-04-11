using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Configurations;

public class EscalationLevelConfiguration : IEntityTypeConfiguration<EscalationLevel>
{
    public void Configure(EntityTypeBuilder<EscalationLevel> builder)
    {
        builder.ToTable("EscalationLevels");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.HasIndex(e => new { e.TenantId, e.SectorId, e.Order }).IsUnique();

        builder.HasOne(e => e.Tenant).WithMany(t => t.EscalationLevels).HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Sector).WithMany().HasForeignKey(e => e.SectorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.Responsibles).WithOne(r => r.EscalationLevel).HasForeignKey(r => r.EscalationLevelId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class EscalationLevelResponsibleConfiguration : IEntityTypeConfiguration<EscalationLevelResponsible>
{
    public void Configure(EntityTypeBuilder<EscalationLevelResponsible> builder)
    {
        builder.ToTable("EscalationLevelResponsibles");
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => new { r.EscalationLevelId, r.UserId }).IsUnique();

        builder.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}