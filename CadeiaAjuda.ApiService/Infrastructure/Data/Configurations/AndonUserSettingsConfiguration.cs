using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Configurations;

public class AndonUserSettingsConfiguration : IEntityTypeConfiguration<AndonUserSettings>
{
    public void Configure(EntityTypeBuilder<AndonUserSettings> builder)
    {
        builder.ToTable("AndonUserSettings");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.UserId).IsUnique();

        builder.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(s => s.Area).WithMany().HasForeignKey(s => s.AreaId).OnDelete(DeleteBehavior.SetNull);
    }
}
