using CentralApp.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CentralApp.Database;

/// <summary>
/// EF Core DbContext for the central application.
/// </summary>
public class CentralDbContext : DbContext
{
    /// <summary>
    /// Creates a central DbContext.
    /// </summary>
    /// <param name="options"></param>
    public CentralDbContext(DbContextOptions<CentralDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Products table in the central database.
    /// </summary>
    public DbSet<CentralProduct> Products { get; set; } = null!;

    /// <summary>
    /// Stores table in the central database.
    /// </summary>
    public DbSet<CentralStore> Stores { get; set; } = null!;

    /// <summary>
    /// Config table in the central database.
    /// </summary>
    public DbSet<Config> ConfigEntries { get; set; } = null!;
}