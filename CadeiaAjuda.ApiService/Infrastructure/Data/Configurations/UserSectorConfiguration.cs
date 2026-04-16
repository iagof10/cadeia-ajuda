using CadeiaAjuda.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Configurations;

public class UserSectorConfiguration : IEntityTypeConfiguration<UserSector>
{
    public void Configure(EntityTypeBuilder<UserSector> builder)
    {
        builder.ToTable("UserSectors");
        builder.HasKey(us => new { us.UserId, us.SectorId });

        builder.HasOne(us => us.User)
            .WithMany(u => u.UserSectors)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(us => us.Sector)
            .WithMany(s => s.UserSectors)
            .HasForeignKey(us => us.SectorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
