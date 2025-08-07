using Microsoft.Extensions.Logging;

using Moq;

using MyApp.Application.Interfaces;
using MyApp.Application.UseCases;
using MyApp.Domain.Entities;

namespace MyApp.Test.Unit;

public class AddCategoryAndProductUnitTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ILogger<AddCategoryAndProductCommandHandler>> _mockLogger;
    private readonly AddCategoryAndProductCommandHandler _handler;

    public AddCategoryAndProductUnitTests()
    {
        // Tạo các mock cho Unit of Work và các repository
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<AddCategoryAndProductCommandHandler>>();

        // Cấu hình mock UnitOfWork để trả về các repository đã mock
        _mockUnitOfWork.Setup(uow => uow.Categories).Returns(_mockCategoryRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Products).Returns(_mockProductRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).ReturnsAsync(1);

        // Khởi tạo handler với mock UnitOfWork
        _handler = new AddCategoryAndProductCommandHandler(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldAddCategoryAndProduct_WhenCalledWithValidData()
    {
        // Arrange
        var command = new AddCategoryAndProductCommand("Electronics", "Smartphone");
        var categoryId = 1;

        // Cấu hình mock CategoryRepository.AddAsync để trả về một Category có Id
        _mockCategoryRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync(new Category { Id = categoryId, Name = command.CategoryName });

        // Act
        await _handler.HandleAsync(command);

        // Assert
        // 1. Kiểm tra AddAsync của CategoryRepository đã được gọi đúng một lần
        _mockCategoryRepository.Verify(repo => repo.AddAsync(It.Is<Category>(c => c.Name == command.CategoryName)), Times.Once);

        // 2. Kiểm tra AddAsync của ProductRepository đã được gọi đúng một lần
        _mockProductRepository.Verify(repo => repo.AddAsync(It.Is<Product>(p =>
            p.Name == command.ProductName && p.CategoryId == categoryId)), Times.Once);

        // 3. Kiểm tra CompleteAsync của UnitOfWork đã được gọi đúng một lần
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);

        // 4. Đảm bảo Rollback không được gọi
        _mockUnitOfWork.Verify(uow => uow.Rollback(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRollback_WhenExceptionOccurs()
    {
        // Arrange
        var command = new AddCategoryAndProductCommand("Electronics", "Smartphone");

        // Cấu hình mock CategoryRepository.AddAsync để ném ra một exception
        _mockCategoryRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Category>()))
            .ThrowsAsync(new InvalidOperationException("Simulated DB error."));

        // Act & Assert
        // Đảm bảo handler ném ra exception và gọi Rollback
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
        _mockUnitOfWork.Verify(uow => uow.Rollback(), Times.Once);
    }
}
