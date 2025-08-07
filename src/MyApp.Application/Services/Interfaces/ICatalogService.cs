namespace MyApp.Application.Services.Interfaces;

public interface ICatalogService
{
    Task<bool> AddCategoryAndProductAsync(string categoryName, string productName);
}
