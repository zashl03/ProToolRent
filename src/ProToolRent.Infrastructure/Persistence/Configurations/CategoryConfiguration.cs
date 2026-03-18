using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration :IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey(c => c.ParentId);

        builder.Navigation(c => c.Parent);
    }
}
