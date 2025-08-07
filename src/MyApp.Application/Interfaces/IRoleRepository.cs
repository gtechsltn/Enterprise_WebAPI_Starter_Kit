using MyApp.Domain.Entities;

namespace MyApp.Application.Interfaces;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetRoleByNameAsync(string roleName);
}
