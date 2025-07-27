using FoodStore.Data.Models;
using FoodStore.Data;
using FoodStore.Services.Core;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Tests.SupplierServiceTests
{
    public class SupplierServiceTests
    {
        private FoodStoreDbContext dbContext;
        private Mock<UserManager<ApplicationUser>> userManagerMock;
        private SupplierService supplierService;

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

            supplierService = new SupplierService(dbContext, userManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task GetSuppliersDropDownAsync_ReturnsAllSuppliers()
        {
            dbContext.Suppliers.Add(new Supplier 
            { 
                Name = "Forest91",
                Phone = "0897564457",
                EmailAddress = "forest91@gmail.com"
            });
            dbContext.Suppliers.Add(new Supplier 
            { 
                Name = "Lina",
                Phone = "088556477",
                EmailAddress = "lina@gmail.com"
            });
            await dbContext.SaveChangesAsync();

            var result = await supplierService.GetSuppliersDropDownAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllSuppliersAsync_ReturnsOnlyNotDeletedSuppliers()
        {
            dbContext.Suppliers.Add(new Supplier 
            { 
                Name = "Toka45",
                Phone = "0897455689",
                EmailAddress = "toka@yahoo.com",
                IsDeleted = false });

            dbContext.Suppliers.Add(new Supplier 
            { 
                Name = "Primex",
                Phone = "0897445089",
                EmailAddress = "primex@mail.bg", 
                IsDeleted = true });

            await dbContext.SaveChangesAsync();

            var result = await supplierService.GetAllSuppliersAsync(pageIndex: 1, pageSize: 10);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Toka45"));
        }

        [Test]
        public async Task AddSupplierAsync_WithValidInput_AddsSupplier()
        {
            var userId = "user123";
            dbContext.Users.Add(new ApplicationUser { Id = userId });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await supplierService.AddSupplierAsync(userId, new AddSupplierInputModel
            {
                Name = "Vinex",
                Phone = "0898605987",
                EmailAddress = "vinex@gmail.com"
            });

            Assert.IsTrue(result);
            Assert.That(dbContext.Suppliers.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetSupplierForEditingAsync_WithValidId_ReturnsModel()
        {
            var userId = "user456";
            var supplier = new Supplier 
            { 
                Id = 10, 
                Name = "Fortuna", 
                Phone = "0877346908", 
                EmailAddress = "fortuna@mail.bg" 
            };

            dbContext.Suppliers.Add(supplier);
            dbContext.Users.Add(new ApplicationUser { Id = userId });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await supplierService.GetSupplierForEditingAsync(userId, 10);

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Fortuna"));
        }

        [Test]
        public async Task EditSupplierAsync_WithValidModel_UpdatesSupplier()
        {
            var userId = "user789";
            var supplier = new Supplier 
            { 
                Id = 20, 
                Name = "Planta", 
                Phone = "0876899654", 
                EmailAddress = "info@planta.com" 
            };

            dbContext.Suppliers.Add(supplier);
            dbContext.Users.Add(new ApplicationUser { Id = userId });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var update = new EditSupplierInputModel
            {
                Id = 20,
                Name = "Planta 45",
                Phone = "0876899657",
                EmailAddress = "planta@gmail.com"
            };

            var result = await supplierService.EditSupplierAsync(userId, update);

            Assert.IsTrue(result);
            var updated = await dbContext.Suppliers.FindAsync(20);
            Assert.That(updated.Name, Is.EqualTo("Planta 45"));
        }

        [Test]
        public async Task GetSupplierForDeletingAsync_ReturnsViewModel()
        {
            var userId = "user999";
            var supplier = new Supplier 
            { 
                Id = 30, 
                Name = "Rina", 
                Phone = "0876345690", 
                EmailAddress = "rina@info.com" 
            };

            dbContext.Suppliers.Add(supplier);
            dbContext.Users.Add(new ApplicationUser { Id = userId });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await supplierService.GetSupplierForDeletingAsync(userId, 30);

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Rina"));
        }

        [Test]
        public async Task SoftDeleteSupplierAsync_MarksSupplierAsDeleted()
        {
            var userId = "user156";
            var supplier = new Supplier 
            { 
                Id = 40, 
                Name = "Paldi", 
                IsDeleted = false,
                Phone = "0876343590",
                EmailAddress = "paldi@abv.bg"
            };

            dbContext.Suppliers.Add(supplier);
            dbContext.Users.Add(new ApplicationUser { Id = userId });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await supplierService.SoftDeleteSupplierAsync(userId, new SupplierDeleteViewModel
            {
                Id = 40,
                Name = "Paldi"
            });

            Assert.IsTrue(result);
            var updated = await dbContext.Suppliers.FindAsync(40);
            Assert.IsTrue(updated.IsDeleted);
        }
    }
}
