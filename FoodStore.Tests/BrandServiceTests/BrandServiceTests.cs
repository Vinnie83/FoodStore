using FoodStore.Data;
using FoodStore.Data.Models;
using FoodStore.Services.Core;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FoodStore.Tests.BrandServiceTests
{
    public class BrandServiceTests
    {

        private FoodStoreDbContext dbContext;
        private Mock<UserManager<ApplicationUser>> userManagerMock;
        private BrandService brandService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FoodStoreDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

            dbContext = new FoodStoreDbContext(options);

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object,
                null, null, null, null, null, null, null, null);

            brandService = new BrandService(dbContext, userManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task GetBrandsDropDownAsync_ReturnsAllBrands()
        {
            dbContext.Brands.Add(new Brand { Name = "Barilla", CountryOfOrigin = "Italy" });
            dbContext.Brands.Add(new Brand { Name = "Sole Mio", CountryOfOrigin = "Italy" });
            await dbContext.SaveChangesAsync();

            var result = await brandService.GetBrandsDropDownAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllBrandsAsync_ReturnsOnlyNotDeletedBrands()
        {
            dbContext.Brands.Add(new Brand { Name = "Barilla", CountryOfOrigin = "Italy", IsDeleted = false });
            dbContext.Brands.Add(new Brand { Name = "Sole Mio", CountryOfOrigin = "Italy", IsDeleted = true });
            await dbContext.SaveChangesAsync();

            var result = await brandService.GetAllBrandsAsync(pageIndex: 1, pageSize: 10);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Barilla"));
        }

        [Test]
        public async Task AddBrandAsync_WithValidInput_AddsBrand()
        {
            var userId = "user123";
            dbContext.Users.Add(new ApplicationUser { Id = userId, Email = "marin@abv.bg" });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await brandService.AddBrandAsync(userId, new AddBrandInputModel
            {
                Name = "Kubeti",
                CountryOfOrigin = "Bulgaria"
            });

            Assert.IsTrue(result);
            Assert.That(dbContext.Brands.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetBrandForEditingAsync_WithValidId_ReturnsModel()
        {
            var userId = "user456";
            var brand = new Brand { Id = 10, Name = "Green", CountryOfOrigin = "Germany" };
            dbContext.Brands.Add(brand);
            dbContext.Users.Add(new ApplicationUser { Id = userId });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await brandService.GetBrandForEditingAsync(userId, 10);

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Green"));
        }

        [Test]
        public async Task EditBrandAsync_WithValidModel_UpdatesBrand()
        {
            var userId = "user789";
            var brand = new Brand { Id = 20, Name = "Nivea", CountryOfOrigin = "Germany" };
            dbContext.Brands.Add(brand);
            dbContext.Users.Add(new ApplicationUser { Id = userId });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var updatedModel = new EditBrandInputModel
            {
                Id = 20,
                Name = "Loreal",
                CountryOfOrigin = "Paris"
            };

            var result = await brandService.EditBrandAsync(userId, updatedModel);

            Assert.IsTrue(result);
            var updated = await dbContext.Brands.FindAsync(20);
            Assert.That(updated.Name, Is.EqualTo("Loreal"));
        }

        [Test]
        public async Task GetBrandForDeletingAsync_ReturnsViewModel()
        {
            var userId = "user999";
            var brand = new Brand { Id = 30, Name = "Bojentsi", CountryOfOrigin = "Bulgaria" };
            dbContext.Brands.Add(brand);
            dbContext.Users.Add(new ApplicationUser { Id = userId });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await brandService.GetBrandForDeletingAsync(userId, 30);

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Bojentsi"));
        }

        [Test]
        public async Task SoftDeleteBrandAsync_MarksBrandAsDeleted()
        {
            var userId = "user156";
            var brand = new Brand { Id = 40, Name = "Kalamata", CountryOfOrigin = "Greece", IsDeleted = false };
            dbContext.Brands.Add(brand);
            dbContext.Users.Add(new ApplicationUser { Id = userId });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await brandService.SoftDeleteBrandAsync(userId, new BrandDeleteViewModel
            {
                Id = 40,
                Name = "Kalamata"
            });

            Assert.IsTrue(result);
            var updated = await dbContext.Brands.FindAsync(40);
            Assert.IsTrue(updated.IsDeleted);
        }
    
     }
}