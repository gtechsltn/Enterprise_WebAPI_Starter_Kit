using MyApp.Domain.Entities;

namespace MyApp.Application.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
}