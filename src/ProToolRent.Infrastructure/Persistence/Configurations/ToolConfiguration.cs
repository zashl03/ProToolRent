using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Infrastructure.Persistence.Configurations;

public class ToolConfiguration : IEntityTypeConfiguration<Tool>
{
    public void Configure(EntityTypeBuilder<Tool> builder)
    {
        builder.ToTable("Tools");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Price)
            .IsRequired();

        builder.ComplexProperty(t => t.Specification, spec =>
        {
            spec.Property(s => s.Brand)
                .HasColumnName("Brand")
                .IsRequired();
            spec.Property(s => s.Name)
                .HasColumnName("Name")
                .IsRequired();
            spec.Property(s => s.Power)
                .HasColumnName("Power")
                .IsRequired();
        });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .IsRequired();

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(t => t.CategoryId)
            .IsRequired();
    }
}
