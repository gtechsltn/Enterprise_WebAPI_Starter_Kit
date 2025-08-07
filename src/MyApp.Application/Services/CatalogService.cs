using MyApp.Application.Interfaces;
using MyApp.Application.Services.Interfaces;
using MyApp.Domain.Entities;

namespace MyApp.Application.Services;

public class CatalogService : ICatalogService
{
    private readonly IUnitOfWork _unitOfWork;

    public CatalogService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> AddCategoryAndProductAsync(string categoryName, string productName)
    {
        try
        {
            // Bước 1: Tạo Category và lấy Id tự tăng
            var newCategory = new Category { Name = categoryName, Description = "Default description" };
            var addedCategory = await _unitOfWork.Categories.AddAsync(newCategory);

            // Bước 2: Tạo Product và sử dụng Id của Category vừa tạo
            var newProduct = new Product
            {
                Name = productName,
                Price = 0, // Giá trị mặc định
                CategoryId = addedCategory.Id, // Sử dụng Id tự tăng
            };
            await _unitOfWork.Products.AddAsync(newProduct);

            // Bước 3: Commit toàn bộ giao dịch
            await _unitOfWork.CompleteAsync();

            return true;
        }
        catch (Exception)
        {
            // Xử lý lỗi, có thể log lỗi ở đây
            _unitOfWork.Rollback();
            return false;
        }
    }
}
