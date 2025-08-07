using System.Reflection;

using Bogus;

using Microsoft.EntityFrameworkCore;

using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    private static readonly string[] RoleList = ["Admin", "User"];

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Áp dụng tất cả các cấu hình từ assembly hiện tại
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Tạo dữ liệu giả
        var roles = new List<Role>
        {
            new Role { Id = 1, RoleName = RoleList[0] },
            new Role { Id = 2, RoleName = RoleList[1] }
        };

        var userFaker = new Faker<User>()
            .RuleFor(u => u.Id, f => f.IndexFaker + 1)
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Username))
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
            .RuleFor(u => u.FullName, f => f.Name.FullName())
            .RuleFor(u => u.CreatedAt, f => f.Date.Past(1))
            .RuleFor(u => u.IsActive, f => f.Random.Bool());

        var users = userFaker.Generate(10); // Tạo 10 người dùng giả

        var userRoles = new List<UserRole>();
        var random = new Randomizer();

        foreach (var user in users)
        {
            var roleName = random.ListItem(RoleList);
            var role = roles.First(r => r.RoleName == roleName);

            userRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });
        }

        // Thêm dữ liệu mẫu
        modelBuilder.Entity<Role>().HasData(roles);
        modelBuilder.Entity<User>().HasData(users);
        modelBuilder.Entity<UserRole>().HasData(userRoles);
    }
}
