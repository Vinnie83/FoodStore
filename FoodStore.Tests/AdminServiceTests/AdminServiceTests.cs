using FoodStore.Data;
using FoodStore.Data.Models;
using FoodStore.Services.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FoodStore.Tests.AdminServiceTests
{
    public class AdminServiceTests
    {
        private FoodStoreDbContext _context;
        private AdminService _adminService;
        private UserManager<ApplicationUser> _userMgr;
        private IServiceProvider _serviceProvider;

        [SetUp]
        public async Task Setup()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddDbContext<FoodStoreDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<FoodStoreDbContext>()
                    .AddDefaultTokenProviders();

            _serviceProvider = services.BuildServiceProvider();

            _context = _serviceProvider.GetRequiredService<FoodStoreDbContext>();
            _userMgr = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            _adminService = new AdminService(_userMgr);

            await SeedUsersAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _userMgr.Dispose(); 

        }

        private async Task SeedUsersAsync()
        {
            var roles = new[] { "Role_user1", "Role_user2", "Role_user3" };

            // Create roles first
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role {roleName}");
                    }
                }
            }
            var users = new List<ApplicationUser>
            {
            new ApplicationUser { UserName = "user1", Email = "user1@foodstore.com" },
            new ApplicationUser { UserName = "user2", Email = "user2@foodstore.com" },
            new ApplicationUser { UserName = "user3", Email = "user3@foodstore.com" }
            };

            foreach (var user in users)
            {
                var createResult = await _userMgr.CreateAsync(user, "Password123!");
                if (!createResult.Succeeded)
                    throw new System.Exception("Failed to create user in seed");

                await _userMgr.AddToRoleAsync(user, $"Role_{user.UserName}");
            }
        }

        [Test]
        public async Task GetAllUsersAsync_ReturnsPaginatedUsersWithRoles()
        {
            var result = await _adminService.GetAllUsersAsync(pageIndex: 1, pageSize: 2);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.PageIndex, Is.EqualTo(1));
            Assert.That(result.HasNextPage, Is.True);

            var firstUser = result[0];
            Assert.That(firstUser.UserName, Is.EqualTo("user1"));
            Assert.That(firstUser.Roles, Contains.Item("Role_user1"));
        }


    }
}
