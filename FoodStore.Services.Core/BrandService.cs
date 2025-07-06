using FoodStore.Data;
using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Services.Core
{
    public class BrandService : IBrandService
    {
        private readonly FoodStoreDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public BrandService(FoodStoreDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;    
            this.userManager = userManager;
        }


        public async Task<IEnumerable<AddBrandDropDownMenu>> GetBrandsDropDownAsync()
        {
            IEnumerable<AddBrandDropDownMenu> addBrandsAsDropDown = await this.dbContext
                .Brands
                .AsNoTracking()
                .Select(b => new AddBrandDropDownMenu()
                {
                    Id = b.Id,
                    BrandName = b.Name,
                })
                .ToArrayAsync();

            return addBrandsAsDropDown;
        }

        public async Task<IEnumerable<BrandViewModel>> GetAllBrandsAsync()
        {
            var allBrands = await this.dbContext
                .Brands
                .AsNoTracking()
                .Select(b => new BrandViewModel()
                {
                    Name = b.Name,
                    CountryOfOrigin = b.CountryOfOrigin
                })
                .ToListAsync();

            return allBrands;
        }


        public async Task<bool> AddBrandAsync(string userId, AddBrandInputModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.CountryOfOrigin))
                return false;

            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            bool exists = await dbContext.Brands
                .AnyAsync(b => b.Name.ToLower() == model.Name.ToLower());

            if (exists)
                return false;

            if (user == null)
            {
                return false;
            }

            var newBrand = new Brand
            {
                Name = model.Name,
                CountryOfOrigin = model.CountryOfOrigin
            };

            await dbContext.Brands.AddAsync(newBrand);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
