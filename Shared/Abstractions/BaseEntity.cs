namespace Shared.Abstractions;

/// <summary>
/// Base type for database entities.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Entity identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Date and time when the entity was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the entity was last updated (UTC).
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
