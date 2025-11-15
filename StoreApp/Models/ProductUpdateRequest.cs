namespace StoreApp.Models;

public class ProductUpdateRequest
{
    public string? Name { get; set; }
    
    public string? Description { get; set; }
    
    public decimal Price { get; set; }
    
    public decimal MinPrice { get; set; }
}