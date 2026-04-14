using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.TradeName).HasMaxLength(200);
        builder.Property(t => t.Cnpj).IsRequired().HasMaxLength(18);
        builder.Property(t => t.Email).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Phone).HasMaxLength(20);
        builder.Property(t => t.Identifier).IsRequired().HasMaxLength(100);
        builder.Property(t => t.StandardUserLimit);
        builder.Property(t => t.AndonUserLimit);
        builder.Property(t => t.ManagerUserLimit);
        builder.Property(t => t.AdministratorUserLimit);
        builder.HasIndex(t => t.Cnpj).IsUnique();
        builder.HasIndex(t => t.Identifier).IsUnique();
    }
}