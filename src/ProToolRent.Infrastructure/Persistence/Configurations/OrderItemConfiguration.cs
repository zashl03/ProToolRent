

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProToolRent.Domain.Entities;

namespace ProToolRent.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.CreatedDate)
            .IsRequired();

        builder.Property(oi => oi.EndDate)
            .HasDefaultValue(DateTime.MaxValue);

        builder.Property(oi => oi.Cost)
            .IsRequired();

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.HasOne(oi => oi.Tool)
            .WithMany()
            .HasForeignKey(oi => oi.ToolId);

        builder.Navigation(oi => oi.Tool);

        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        builder.Navigation(oi => oi.Order);
    }
}
