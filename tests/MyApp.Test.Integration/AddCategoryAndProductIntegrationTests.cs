using Microsoft.EntityFrameworkCore;

using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories;

namespace MyApp.Test.Integration;

public class AddCategoryAndProductIntegrationTests : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CategoryRepository _categoryRepository;
    private readonly ProductRepository _productRepository;

    public AddCategoryAndProductIntegrationTests(TestFixture fixture)
    {
        _dbContext = fixture.GetContext();
        _categoryRepository = new CategoryRepository(_dbContext);
        _productRepository = new ProductRepository(_dbContext);
    }

    [Fact]
    public async Task AddCategoryAndProductAsync_ShouldSaveBothToDatabase()
    {
        // Arrange
        var newCategory = new Category { Name = "Electronics", Description = "Electronic devices" };
        var newProduct = new Product { Name = "Smartphone", Description = "Smartphone devices", Price = 999.99m };

        // Act
        // Bước 1: Thêm Category vào DbContext trước
        await _categoryRepository.AddAsync(newCategory);

        // Bước 2: Sau khi AddAsync, newCategory đã có Id được EF Core gán
        // Gán Category cho Product
        newProduct.Category = newCategory;

        // Bước 3: Thêm Product vào DbContext
        await _productRepository.AddAsync(newProduct);

        // Bước 4: Gọi SaveChangesAsync() để commit cả hai
        await _dbContext.SaveChangesAsync();

        // Giả lập logic trong Unit of Work
        // Lưu ý: Trong ứng dụng thật, bạn sẽ sử dụng Unit of Work thay vì gọi trực tiếp SaveChanges
        await _dbContext.SaveChangesAsync();

        // Assert (sample data)
        Assert.True(_dbContext.Users.ToList().Count == 10);
        Assert.True(_dbContext.Roles.ToList().Count == 2);
        Assert.True(_dbContext.UserRoles.ToList().Count == 10);

        // Assert
        var savedCategory = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics");
        var savedProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.Name == "Smartphone");

        Assert.NotNull(savedCategory);
        Assert.Equal("Electronics", savedCategory.Name);
        Assert.NotNull(savedProduct);
        Assert.Equal("Smartphone", savedProduct.Name);
    }
}