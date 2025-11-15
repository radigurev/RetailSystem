using Microsoft.EntityFrameworkCore;
using StoreApp.Database.Models;

namespace StoreApp.Database;

public class StoreDbContext : DbContext
{
    /// <summary>
    /// Creates a store DbContext.
    /// </summary>
    /// <param name="options"></param>
    public StoreDbContext(DbContextOptions<StoreDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Products table in the store database.
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// Configuration entries for the store application.
    /// </summary>
    public DbSet<Config> ConfigEntries { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
 
