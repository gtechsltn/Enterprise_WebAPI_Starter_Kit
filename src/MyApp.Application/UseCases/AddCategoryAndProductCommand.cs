namespace MyApp.Application.UseCases;

public class AddCategoryAndProductCommand
{
    public string CategoryName { get; }
    public string ProductName { get; }

    public AddCategoryAndProductCommand(string categoryName, string productName)
    {
        CategoryName = categoryName;
        ProductName = productName;
    }
}
