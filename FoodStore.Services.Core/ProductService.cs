using FoodStore.Data;
using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static FoodStore.GCommon.ValidationConstants;
using Brand = FoodStore.Data.Models.Brand;
using Category = FoodStore.Data.Models.Category;
using Product = FoodStore.Data.Models.Product;
using Supplier = FoodStore.Data.Models.Supplier;

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


        public async Task<PaginatedList<ProductViewModel>> GetByCategoryAsync(string categoryName, int pageIndex, int pageSize)
        {
            var productsByCategory = this.dbContext
                .Products
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => !p.IsDeleted &&
                                   p.Category != null &&
                                   p.Category.Name.ToLower() == categoryName.ToLower())
                .Select(p => new ProductViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl ?? $"~/images/{NoImageUrl}",
                    Price = p.Price,
                    StockQuantity = p.Quantity,
                    UnitQuantity = "1"

                });

            return await PaginatedList<ProductViewModel>.CreateAsync(productsByCategory, pageIndex, pageSize) ;
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

        public async Task<EditProductInputModel?> GetProductForEditingAsync (string userId, int? id)
        {
            EditProductInputModel? model = null!;

            if (id != null)
            {

                ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

                Product? editModel = await this.dbContext
                    .Products
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.Id == id);

                if (editModel != null && userId.ToLower() == user?.Id.ToLower())
                {
                    model = new EditProductInputModel()
                    {
                        Id = editModel.Id,
                        Name = editModel.Name,
                        ImageUrl = editModel.ImageUrl,
                        Price = editModel.Price,
                        CategoryId = editModel.CategoryId,
                        BrandId = editModel.BrandId,
                        SupplierId = editModel.SupplierId,
                        StockQuantity = editModel.Quantity,
                    };
                    
                }
            
            }
            return model;
        }

        public async Task<bool> PersistUpdatedProductAsync(string userId, EditProductInputModel inputModel)
        {
            bool result = false;

            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            Product? updatedProduct = await this.dbContext
                 .Products
                 .FindAsync(inputModel.Id);

            Category? catReference = await this.dbContext
                .Categories
                .FindAsync(inputModel.CategoryId);

            Brand? brandReference = await this.dbContext
                .Brands
                .FindAsync(inputModel.BrandId);

            Supplier? supplierReference = await this.dbContext
                .Suppliers 
                .FindAsync(inputModel.SupplierId);

            if ((user != null) && (catReference != null) && (brandReference != null)
                && (supplierReference != null) && (updatedProduct != null))
            {
                updatedProduct.Name = inputModel.Name;
                updatedProduct.ImageUrl = inputModel.ImageUrl;
                updatedProduct.Price = inputModel.Price;
                updatedProduct.CategoryId = inputModel.CategoryId;
                updatedProduct.BrandId = inputModel.BrandId;
                updatedProduct.SupplierId = inputModel.SupplierId;
                updatedProduct.Quantity = inputModel.StockQuantity;

                await this.dbContext.SaveChangesAsync();

                result = true;
            }

            return result;
        }

        public async Task<ProductDeleteInputModel?> GetProductForDeletingAsync(string userId, int? productId)
        {
            ProductDeleteInputModel? deleteModel = null;

            if (productId != null)
            {
                ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

                Product? deleteProductModel = await this.dbContext
                    .Products
                    .Include(p => p.Category)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.Id == productId);

                if ((deleteProductModel != null) && (userId.ToLower() == user?.Id.ToLower()))
                {
                    deleteModel = new ProductDeleteInputModel()
                    {
                        Id = deleteProductModel.Id,
                        Name = deleteProductModel.Name,
                        CategoryName = deleteProductModel.Category.Name,
                        ImageUrl = deleteProductModel.ImageUrl,
                        Price = deleteProductModel.Price,
                    };
                }
                
            }

            return deleteModel;
        }

        public async Task<bool> SoftDeleteProductAsync(string userId, ProductDeleteInputModel inputModel)
        {
            bool result = false;

            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            Product? deletedProduct = await this.dbContext
                .Products
                .FindAsync(inputModel.Id);

            if ((user != null) && (deletedProduct != null)) 
            {
                deletedProduct.IsDeleted = true;

                await this.dbContext.SaveChangesAsync();

                result = true;
            }

            return result;
        }

        public async Task<IEnumerable<ProductSearchResultViewModel>> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Enumerable.Empty<ProductSearchResultViewModel>();
            }

            query = query.ToLower();

            return await this.dbContext
                .Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Where(p => p.Name.ToLower().Contains(query) ||
                       p.Category.Name.ToLower().Contains(query) ||
                       p.Supplier.Name.ToLower().Contains(query) ||
                       p.Brand.Name.ToLower().Contains(query)             
                )
                .Select(p => new ProductSearchResultViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category.Name,
                    Brand = p.Brand.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                })
                .ToArrayAsync();
        }
    }
}
