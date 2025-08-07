using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // Đặt tên bảng là "UserRoles"
        builder.ToTable("UserRoles");

        // Cấu hình khóa chính composite
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });
    }
}
