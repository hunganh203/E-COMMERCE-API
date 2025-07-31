using Application.Constants;
using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Persistence.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task DeleteById(Guid key)
        {
            await using var transaction = await this._dbContext.Database.BeginTransactionAsync();

            var product = await this._dbContext.Products.AsQueryable()
                .Include(p => p.ProductAttributes)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductRelateds)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(x => x.Id == key);

            if (product == null)
                throw new Exception(CustomResponseMessage.ProductDoesNotExist);

            this._dbContext.ProductAttributes.RemoveRange(product.ProductAttributes);
            this._dbContext.ProductImages.RemoveRange(product.ProductImages);
            this._dbContext.ProductRelateds.RemoveRange(product.ProductRelateds);
            this._dbContext.Reviews.RemoveRange(product.Reviews);
            this._dbContext.Products.Remove(product);

            await this._dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}