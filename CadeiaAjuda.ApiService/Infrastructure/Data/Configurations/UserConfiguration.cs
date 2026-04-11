using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Phone).HasMaxLength(20);
        builder.Property(u => u.Login).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
        builder.HasIndex(u => new { u.TenantId, u.Login }).IsUnique();
        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();

        builder.HasOne(u => u.Tenant).WithMany(t => t.Users).HasForeignKey(u => u.TenantId).OnDelete(DeleteBehavior.Restrict);
    }
}