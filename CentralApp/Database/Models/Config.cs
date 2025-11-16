using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;

namespace CentralApp.Database.Models;

/// <summary>
/// Configuration entry for the central application.
/// </summary>
[Table("Config")]
[Index(nameof(Key), IsUnique = true)]
public class Config : BaseEntity
{
    /// <summary>
    /// Configuration key.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Configuration value.
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Optional description.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}