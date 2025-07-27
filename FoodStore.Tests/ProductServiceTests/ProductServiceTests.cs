using FoodStore.Data.Models;
using FoodStore.Data;
using FoodStore.Services.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using FoodStore.ViewModels.Admin;

namespace FoodStore.Tests.ProductServiceTests
{
    public class ProductServiceTests
    {
        private FoodStoreDbContext _context;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private ProductService _productService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FoodStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "FoodStoreTestDb")
                .Options;

            _context = new FoodStoreDbContext(options);


            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _productService = new ProductService(_context, _userManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetByCategoryAsync_ReturnsProductsInCategory()
        {

            var category = new Category { Id = 1, Name = "Fruits" };
            _context.Categories.Add(category);
            _context.Products.AddRange(
                new Product { Id = 1, Name = "Apple", Category = category, Price = 1, Quantity = 10 },
                new Product { Id = 2, Name = "Banana", Category = category, Price = 2, Quantity = 5 },
                new Product { Id = 3, Name = "Carrot", Category = new Category { Id = 2, Name = "Vegetables" }, Price = 3, Quantity = 7 }
            );
            await _context.SaveChangesAsync();


            var result = await _productService.GetByCategoryAsync("fruits", 1, 10);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Apple"));
            Assert.That(result[1].Name, Is.EqualTo("Banana"));
        }

        [Test]
        public async Task GetProductByIdAsync_ReturnsProductDetails()
        {

            var brand = new Brand { Id = 1, Name = "Dole", CountryOfOrigin = "Ecuador" };
            var category = new Category { Id = 1, Name = "Fruits" };
            var product = new Product { Id = 1, Name = "Apple", Brand = brand, Category = category, Price = 1.5m, Quantity = 10 };
            _context.Brands.Add(brand);
            _context.Categories.Add(category);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var details = await _productService.GetProductByIdAsync(1);

            Assert.IsNotNull(details);
            Assert.That(details.Name, Is.EqualTo("Apple"));
            Assert.That(details.Brand, Is.EqualTo("Dole"));
            Assert.That(details.Category, Is.EqualTo("Fruits"));

        }

        [Test]
        public async Task AddProductAsync_ReturnsFalse_WhenUserNotFound()
        {

            _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            var result = await _productService.AddProductAsync("nonexistent-user", new ViewModels.Admin.ProductCreateInputModel());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddProductAsync_ReturnsFalse_WhenCategoryNotFound()
        {

            var user = new ApplicationUser { Id = "user1" };
            _userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).ReturnsAsync(user);

            var result = await _productService.AddProductAsync(user.Id, new ViewModels.Admin.ProductCreateInputModel
            {
                CategoryId = 999 
            });

            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddProductAsync_ReturnsTrue_AndAddsProduct()
        {

            var user = new ApplicationUser { Id = "user1" };
            _userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).ReturnsAsync(user);

            var category = new Category { Id = 1, Name = "Fruits" };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var model = new ViewModels.Admin.ProductCreateInputModel
            {
                Name = "Apple",
                Price = 1.5m,
                CategoryId = category.Id,
                BrandId = 1,
                SupplierId = 1,
                StockQuantity = 10,
                ImageUrl = "apple.jpg"
            };


            var result = await _productService.AddProductAsync(user.Id, model);

            Assert.IsTrue(result);

            var productInDb = _context.Products.FirstOrDefault(p => p.Name == "Apple");
            Assert.IsNotNull(productInDb);
            Assert.That(productInDb.Price, Is.EqualTo(model.Price));
        }

        [Test]
        public async Task GetCategoryNameByIdAsync_ReturnsCorrectName()
        {
            var category = new Category { Id = 1, Name = "Bakery" };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await _productService.GetCategoryNameByIdAsync(1);

            Assert.That(result, Is.EqualTo("Bakery"));
        }

        [Test]
        public async Task GetProductForEditingAsync_ReturnsModel_WhenUserOwnsProduct()
        {
            var user = new ApplicationUser { Id = "user1", UserName = "user1@foodstore.com" };
            await _context.Users.AddAsync(user);

            var product = new Product
            {
                Id = 1,
                Name = "Cheese Bree",
                CategoryId = 1,
                BrandId = 1,
                SupplierId = 1,
                Quantity = 10
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(x => x.FindByIdAsync("user1")).ReturnsAsync(user);

            var result = await _productService.GetProductForEditingAsync("user1", 1);

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Cheese Bree"));
        }

        [Test]
        public async Task PersistUpdatedProductAsync_UpdatesProduct_WhenValid()
        {
            var user = new ApplicationUser { Id = "user1" };
            await _context.Users.AddAsync(user);
            var product = new Product
            {
                Id = 1,
                Name = "Bob Krina",
                CategoryId = 1,
                BrandId = 1,
                SupplierId = 1,
                Quantity = 5
            };

            await _context.Products.AddAsync(product);
            await _context.Categories.AddAsync(new Category { Id = 1, Name = "Package foods" });
            await _context.Brands.AddAsync(new Brand { Id = 1, Name = "Krina", CountryOfOrigin = "Bulgaria" });
            await _context.Suppliers.AddAsync(new Supplier 
            { Id = 1, Name = "MaxTrade", EmailAddress = "maxtrade@abv.bg", Phone = "0898765465" });
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(x => x.FindByIdAsync("user1")).ReturnsAsync(user);

            var model = new EditProductInputModel
            {
                Id = 1,
                Name = "Bob Krina 500",
                CategoryId = 1,
                BrandId = 1,
                SupplierId = 1,
                Price = 9.99m,
                ImageUrl = "test.jpg",
                StockQuantity = 20
            };

            var result = await _productService.PersistUpdatedProductAsync("user1", model);

            Assert.IsTrue(result);
            Assert.That(_context.Products.First().Name, Is.EqualTo("Bob Krina 500"));
        }

        [Test]
        public async Task GetProductForDeletingAsync_ReturnsModel_WhenProductExists()
        {
            var user = new ApplicationUser { Id = "user1" };
            await _context.Users.AddAsync(user);

            var category = new Category { Id = 1, Name = "Beverages" };
            var product = new Product
            {
                Id = 1,
                Name = "Fanta",
                Category = category,
                CategoryId = 1,
                Quantity = 10
            };

            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(x => x.FindByIdAsync("user1")).ReturnsAsync(user);

            var result = await _productService.GetProductForDeletingAsync("user1", 1);

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Fanta"));
            Assert.That(result.CategoryName, Is.EqualTo("Beverages"));
        }

        [Test]
        public async Task SoftDeleteProductAsync_SetsIsDeletedTrue_WhenValid()
        {
            var user = new ApplicationUser { Id = "user1" };
            await _context.Users.AddAsync(user);

            await _context.Categories.AddAsync(new Category { Id = 1, Name = "Drinks" });
            await _context.Brands.AddAsync(new Brand
            {
                Id = 1,
                Name = "Coca Cola",
                CountryOfOrigin = "USA"
            });
            await _context.Suppliers.AddAsync(new Supplier
            {
                Id = 1,
                Name = "MegaSupplier",
                Phone = "08999754657",
                EmailAddress = "mega@abv.bg"
            });

            var product = new Product
            {
                Id = 1,
                Name = "Coca Cola",
                IsDeleted = false,
                CategoryId = 1,
                BrandId = 1,
                SupplierId = 1,
                Quantity = 10,
                Price = 1.99m
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(x => x.FindByIdAsync("user1")).ReturnsAsync(user);

            var input = new ProductDeleteInputModel { Id = 1 };

            var result = await _productService.SoftDeleteProductAsync("user1", input);

            var deletedProduct = await _context.Products
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == 1);

            Assert.IsTrue(result);
            Assert.IsNotNull(deletedProduct);
            Assert.IsTrue(deletedProduct.IsDeleted);
        }

        [Test]
        public async Task SearchProductsAsync_ReturnsMatchingProducts()
        {
            var category = new Category { Id = 1, Name = "Dairy" };
            var brand = new Brand { Id = 1, Name = "BestBrand", CountryOfOrigin = "Bulgaria" };
            var supplier = new Supplier { Id = 1, Name = "TopSupplier", Phone = "0898768906", EmailAddress = "best@gmail.com"  };

            await _context.Categories.AddAsync(category);
            await _context.Brands.AddAsync(brand);
            await _context.Suppliers.AddAsync(supplier);
            await _context.Products.AddAsync(new Product
            {
                Id = 1,
                Name = "Milk",
                Category = category,
                Brand = brand,
                Supplier = supplier
            });

            await _context.SaveChangesAsync();

            var results = await _productService.SearchProductsAsync("milk");

            Assert.IsNotEmpty(results);
            Assert.That(results.First().Name, Is.EqualTo("Milk"));
        }

    }
}
