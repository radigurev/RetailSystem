using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;

namespace CentralApp.Database.Models;

/// <summary>
/// Product entity stored in the central database.
/// </summary>
[Table("Products")]
[Index(nameof(SourceStoreId), nameof(Name), IsUnique = true)]
public class CentralProduct : BaseEntity
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

    /// <summary>
    /// Identifier of the source store.
    /// </summary>
    [Required]
    public Guid SourceStoreId { get; set; }

    /// <summary>
    /// Navigation to the source store.
    /// </summary>
    [ForeignKey(nameof(SourceStoreId))]
    public CentralStore? SourceStore { get; set; }
}