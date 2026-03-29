using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProToolRent.Domain.Entities;

namespace ProToolRent.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(up => up.Id);

        builder.Property(up => up.FirstName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(up => up.LastName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(up => up.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(up => up.Organization)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(up => up.Phone)
            .IsRequired()
            .HasMaxLength(50);
    }
}
