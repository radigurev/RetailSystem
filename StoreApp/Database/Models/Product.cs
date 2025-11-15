using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Abstractions;

namespace StoreApp.Database.Models;

/// <summary>
/// Product entity stored in the store database.
/// </summary>
[Table("StoreProducts")]
public class Product : BaseEntity
{
    /// <summary>
    /// Product name.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Product price.
    /// </summary>
    [Column(TypeName = "money")]
    public decimal Price { get; set; }

    /// <summary>
    /// Minimum allowed price.
    /// </summary>
    [Column(TypeName = "money")]
    public decimal MinPrice { get; set; }
}