using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Configurations.Relationship;

public class ProductRelationshipConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Cấu hình mối quan hệ một-nhiều
        // Một Product có một Category
        // Một Category có nhiều Products
        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId);
    }
}