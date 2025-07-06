using FoodStore.Data;
using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static FoodStore.GCommon.ValidationConstants;
using Category = FoodStore.Data.Models.Category;
using Product = FoodStore.Data.Models.Product;

namespace FoodStore.Services.Core
{
    public class ProductService : IProductService
    {
        private readonly FoodStoreDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public ProductService(FoodStoreDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
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

        public async Task<ProductDetailsViewModel> GetProductByIdAsync(int productId)
        {
            ProductDetailsViewModel? productDetails = await this.dbContext
                .Products
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .Select(p => new ProductDetailsViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl ?? $"~/images/{NoImageUrl}",
                    Price = p.Price,
                    StockQuantity = p.Quantity,
                    UnitQuantity = "1",
                    Brand = p.Brand.Name,
                    Category = p.Category.Name
                })
                .FirstOrDefaultAsync();

            return productDetails;

        }

        public async Task<bool> AddProductAsync(string userId, ProductCreateInputModel model)
        {
            bool operResult = false;

            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            Category? catReference = await this.dbContext.Categories.FindAsync(model.CategoryId);

            if ((user != null) && (catReference != null)) 
            {
                Product product = new Product()
                {
                    Name = model.Name,
                    ImageUrl = model.ImageUrl,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    BrandId = model.BrandId,
                    SupplierId = model.SupplierId,
                    Quantity = model.StockQuantity,

                };

                await this.dbContext.Products.AddAsync(product);    
                await this.dbContext.SaveChangesAsync();

                operResult = true;
            }

            return operResult;
        }

        public async Task<string?> GetCategoryNameByIdAsync(int categoryId)
        {
            return await this.dbContext
            .Categories
            .Where(c => c.Id == categoryId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync();
        }
    }
}
