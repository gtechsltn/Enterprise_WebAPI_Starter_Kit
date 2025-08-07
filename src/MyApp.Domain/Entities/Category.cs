namespace MyApp.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;

    // Navigation property
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
