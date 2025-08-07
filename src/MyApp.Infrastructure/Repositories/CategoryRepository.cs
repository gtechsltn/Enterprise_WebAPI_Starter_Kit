using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Repositories;

using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Persistence;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
}
