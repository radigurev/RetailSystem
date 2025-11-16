using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Enums;
using Shared.Exceptions;
using StoreApp.Abstractions;
using StoreApp.Database;
using StoreApp.Database.Models;

namespace StoreApp.CommonLogic
{
    /// <summary>
    /// EF-based implementation of the store product database service.
    /// </summary>
    public class ProductService(
        StoreDbContext _dbContext) : IProductService
    {

        #region Base DB Service Implementation
        /// <summary>
        /// Gets a single store product matching the predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        public async Task<Product> GetAsync(
            Expression<Func<Product, bool>> predicate,
            CancellationToken cancellationToken)
        {
            Product? entity = await _dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
            
            return entity ?? throw new EntityNotFoundException("Product not found");
        }

        /// <summary>
        /// Gets all store products, optionally filtered by predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        public async Task<IEnumerable<Product>> GetAllAsync(
            Expression<Func<Product, bool>>? predicate,
            CancellationToken cancellationToken)
        {
            IQueryable<Product> query = _dbContext.Products.AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Creates a new store product.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        public async Task<Product> CreateAsync(
            Product entity,
            CancellationToken cancellationToken)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _dbContext.Products.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        /// <summary>
        /// Updates an existing store product.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        public async Task<Product> UpdateAsync(
            Product entity,
            CancellationToken cancellationToken)
        {
            Product? existing = await _dbContext.Products
                .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

            if (existing == null)
                throw new EntityNotFoundException("Product not found");

            entity.UpdatedAt = DateTime.UtcNow;

            _dbContext.Entry(existing).CurrentValues.SetValues(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return existing;
        }

        /// <summary>
        /// Deletes store products matching the predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        public async Task DeleteAsync(
            Expression<Func<Product, bool>> predicate,
            CancellationToken cancellationToken)
        {
            List<Product> entities = await _dbContext.Products
                .Where(predicate)
                .ToListAsync(cancellationToken);

            if (entities.Count == 0)
                throw new EntityNotFoundException("Products not found");

            _dbContext.RemoveRange(entities);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        #endregion

        public async Task<Product> UpsertProduct(
            Expression<Func<Product, bool>> productPredicate,
            Product productEntity,
            CancellationToken cancellationToken)
        {
            Product? product = await _dbContext.Products.FirstOrDefaultAsync(productPredicate, cancellationToken);

            if (product is null)
                return await CreateAsync(productEntity, cancellationToken);
        
            return await UpdateAsync(productEntity, cancellationToken);        }
    }
}
