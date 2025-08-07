using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using MyApp.Infrastructure.Data;

namespace MyApp.Test.Integration;

public class TestFixture : IDisposable
{
    private readonly IConfigurationRoot _configuration;
    private readonly ApplicationDbContext _dbContext;

    public TestFixture()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .Build();

        var databaseProvider = _configuration.GetValue<string>("DatabaseProvider") ?? string.Empty;
        var connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

        if (string.IsNullOrEmpty(databaseProvider))
        {
            databaseProvider = "InMemory";
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found or is empty in configuration.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        else
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }

        _dbContext = new ApplicationDbContext(optionsBuilder.Options);
        _dbContext.Database.EnsureCreated();
    }

    public ApplicationDbContext GetContext()
    {
        return _dbContext;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}