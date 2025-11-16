namespace StoreApp.Models;

public class ProductCreateRequest
{
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public decimal MinPrice { get; set; }
}