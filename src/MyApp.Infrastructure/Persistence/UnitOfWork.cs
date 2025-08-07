using MyApp.Application.Interfaces;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _context;
    public IProductRepository Products { get; }
    public ICategoryRepository Categories { get; }
    public IUserRepository Users { get; }
    public IRoleRepository Roles { get; }
    public IUserRoleRepository UserRoles { get; }
    public UnitOfWork(
        ApplicationDbContext context,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _context = context;
        Users = userRepository;
        Roles = roleRepository;
        UserRoles = userRoleRepository;
        Products = productRepository;
        Categories = categoryRepository;
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Rollback()
    {
        _context.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}