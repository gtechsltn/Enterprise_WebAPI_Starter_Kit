using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Đặt tên bảng là "Categories"
        builder.ToTable("Categories");

        // Cấu hình khóa chính
        builder.HasKey(c => c.Id);

        // Cấu hình Id là cột tự động tăng
        builder.Property(c => c.Id)
               .ValueGeneratedOnAdd();

        // Cấu hình các thuộc tính khác của Category
        builder.Property(c => c.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(c => c.Description)
               .HasMaxLength(500);
    }
}
