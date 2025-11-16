using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;

namespace CentralApp.Database.Models;

/// <summary>
/// Store entry in the central database.
/// </summary>
[Table("Stores")]
[Index(nameof(Name), IsUnique = true)]
public class CentralStore : BaseEntity
{
    /// <summary>
    /// Store name.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional store code.
    /// </summary>
    [MaxLength(50)]
    public string? Code { get; set; }

    [Required]
    [MaxLength(50)]
    public string RoutingKey { get; set; } = string.Empty;
}