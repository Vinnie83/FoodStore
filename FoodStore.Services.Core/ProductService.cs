using FoodStore.Data;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using Microsoft.EntityFrameworkCore;
using static FoodStore.GCommon.ValidationConstants;

namespace FoodStore.Services.Core
{
    public class ProductService : IProductService
    {
        private readonly FoodStoreDbContext dbContext;
        
        public ProductService(FoodStoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<ProductViewModel>> GetByCategoryAsync(string categoryName)
        {
            IEnumerable<ProductViewModel> productsByCategory = await this.dbContext
                .Products
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => !p.IsDeleted &&
                                   p.Category.Name.ToLower() == categoryName.ToLower())
                .Select(p => new ProductViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl ?? $"~/images/{NoImageUrl}",
                    Price = p.Price,
                    StockQuantity = p.Quantity,
                    UnitQuantity = "1"

                })
                .ToListAsync();

            return productsByCategory;
        }
    }
}
