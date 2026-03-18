using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Fullname)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Organization)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(u => u.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .IsRequired();
    }
}
