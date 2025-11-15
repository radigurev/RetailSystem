using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Abstractions;

/// <summary>
/// Base type for database entities.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Entity identifier.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Date and time when the entity was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the entity was last updated (UTC).
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
