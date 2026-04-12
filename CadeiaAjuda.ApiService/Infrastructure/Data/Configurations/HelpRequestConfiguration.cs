using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Configurations;

public class HelpRequestConfiguration : IEntityTypeConfiguration<HelpRequest>
{
    public void Configure(EntityTypeBuilder<HelpRequest> builder)
    {
        builder.ToTable("HelpRequests");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Code).IsRequired().HasMaxLength(20);
        builder.Property(h => h.Description).HasMaxLength(1000);
        builder.Property(h => h.Status).HasConversion<int>();

        builder.HasIndex(h => new { h.TenantId, h.Code }).IsUnique();

        builder.HasOne(h => h.Tenant).WithMany().HasForeignKey(h => h.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.Sector).WithMany().HasForeignKey(h => h.SectorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.HelpRequestType).WithMany().HasForeignKey(h => h.HelpRequestTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.Area).WithMany().HasForeignKey(h => h.AreaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.RequestedByUser).WithMany().HasForeignKey(h => h.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.ClosedByUser).WithMany().HasForeignKey(h => h.ClosedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
