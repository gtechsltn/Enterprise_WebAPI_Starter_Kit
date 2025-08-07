using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Configurations;
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Đặt tên bảng là "Products"
        builder.ToTable("Products");

        // Cấu hình khóa chính
        builder.HasKey(p => p.Id);

        // Cấu hình Id là cột tự động tăng
        builder.Property(p => p.Id)
               .ValueGeneratedOnAdd();

        // Cấu hình các thuộc tính khác của Product
        builder.Property(p => p.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(p => p.Price)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        // Cấu hình mối quan hệ khóa ngoại
        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId);
    }
}