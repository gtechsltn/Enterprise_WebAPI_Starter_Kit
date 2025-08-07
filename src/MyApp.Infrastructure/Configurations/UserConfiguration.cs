using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Cấu hình tên bảng
        builder.ToTable("Users");

        // Cấu hình khóa chính
        builder.HasKey(u => u.Id);

        // Cấu hình Id là cột tự động tăng
        builder.Property(u => u.Id)
               .ValueGeneratedOnAdd();

        // Cấu hình thuộc tính Username
        builder.Property(u => u.Username)
               .HasMaxLength(50)
               .IsRequired();

        // Cấu hình thuộc tính Email
        builder.Property(u => u.Email)
               .HasMaxLength(100)
               .IsRequired();

        // Cấu hình thuộc tính PasswordHash
        builder.Property(u => u.PasswordHash)
               .HasMaxLength(255)
               .IsRequired();

        // Cấu hình thuộc tính FullName
        builder.Property(u => u.FullName)
               .HasMaxLength(150);

        // Cấu hình thuộc tính CreatedAt
        builder.Property(u => u.CreatedAt)
               .HasDefaultValueSql("GETDATE()"); // Đặt giá trị mặc định là thời gian hiện tại của SQL Server

        // Cấu hình thuộc tính IsActive
        builder.Property(u => u.IsActive)
               .HasDefaultValue(true);

        // Cấu hình mối quan hệ với UserRole (bảng nối)
        builder.HasMany(u => u.UserRoles)
               .WithOne(ur => ur.User)
               .HasForeignKey(ur => ur.UserId);
    }
}
