using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Cấu hình tên bảng
        builder.ToTable("Roles");

        // Cấu hình khóa chính
        builder.HasKey(r => r.Id);

        // Cấu hình Id là cột tự động tăng
        builder.Property(r => r.Id)
               .ValueGeneratedOnAdd();

        // Cấu hình thuộc tính RoleName
        builder.Property(r => r.RoleName)
               .HasMaxLength(50)
               .IsRequired();

        // Cấu hình mối quan hệ với UserRole (bảng nối)
        builder.HasMany(r => r.UserRoles)
               .WithOne(ur => ur.Role)
               .HasForeignKey(ur => ur.RoleId);
    }
}