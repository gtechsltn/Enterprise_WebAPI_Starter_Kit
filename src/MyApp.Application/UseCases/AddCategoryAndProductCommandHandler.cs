using Microsoft.Extensions.Logging;

using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;

namespace MyApp.Application.UseCases;

public class AddCategoryAndProductCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddCategoryAndProductCommandHandler> _logger;
    public AddCategoryAndProductCommandHandler(IUnitOfWork unitOfWork, ILogger<AddCategoryAndProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(AddCategoryAndProductCommand command)
    {
        try
        {
            // Bước 1: Tạo Category và lấy Id tự tăng
            var newCategory = new Category { Name = command.CategoryName, Description = "Default description" };
            var addedCategory = await _unitOfWork.Categories.AddAsync(newCategory);

            // Bước 2: Tạo Product và sử dụng Id của Category vừa tạo
            var newProduct = new Product
            {
                Name = command.ProductName,
                Price = 0, // Giá trị mặc định
                CategoryId = addedCategory.Id, // Sử dụng Id tự tăng
            };
            await _unitOfWork.Products.AddAsync(newProduct);

            // Bước 3: Commit toàn bộ giao dịch
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            _unitOfWork.Rollback();

            throw;
        }
    }
}