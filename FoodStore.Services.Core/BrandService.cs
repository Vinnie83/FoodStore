using FoodStore.Data;
using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
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

        public async Task<PaginatedList<BrandViewModel>> GetAllBrandsAsync(int pageIndex, int pageSize)
        {
            var allBrands = this.dbContext
                .Brands
                .AsNoTracking()
                .Where(b => !b.IsDeleted)
                .Select(b => new BrandViewModel()
                {
                    Id = b.Id,
                    Name = b.Name,
                    CountryOfOrigin = b.CountryOfOrigin
                });

            return await PaginatedList<BrandViewModel>.CreateAsync(allBrands, pageIndex, pageSize);
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

        public async Task<EditBrandInputModel?> GetBrandForEditingAsync(string userId, int? id)
        {
            EditBrandInputModel? editModel = null;

            if (id != null)
            {
                Brand? brand = await this.dbContext
                    .Brands
                    .AsNoTracking()
                    .SingleOrDefaultAsync(b => b.Id == id);

                ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

                if (brand != null && user != null)
                {
                    editModel = new EditBrandInputModel()
                    {
                        Id = brand.Id,
                        Name = brand.Name,
                        CountryOfOrigin = brand.CountryOfOrigin
                    };
                }
            }

            return editModel;
        }

        public async Task<bool> EditBrandAsync(string userId, EditBrandInputModel inputModel)
        {
            bool result = false;

            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            Brand? updatedBrand = await this.dbContext
                .Brands
                .FindAsync(inputModel.Id);

            if ((user != null) && 
                (updatedBrand != null))
            {
                updatedBrand.Name = inputModel.Name;
                updatedBrand.CountryOfOrigin = inputModel.CountryOfOrigin;

                await this.dbContext.SaveChangesAsync();

                result = true;
            }

            return result;
        }

        public async Task<BrandDeleteViewModel?> GetBrandForDeletingAsync(string userId, int? brandId)
        {
            BrandDeleteViewModel? deleteModel = null;

            if (brandId != null)
            {
                Brand? brand = await this.dbContext
                    .Brands
                    .AsNoTracking()
                    .SingleOrDefaultAsync(b => b.Id == brandId);

                ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

                if ((user != null) && (brand != null)) 
                {
                    deleteModel = new BrandDeleteViewModel()
                    {
                        Id = brand.Id,
                        Name = brand.Name,
                        CountryOfOrigin = brand.CountryOfOrigin,
                    };  
                    
                }
            }

            return deleteModel;
        }

        public async Task<bool> SoftDeleteBrandAsync(string userId, BrandDeleteViewModel inputModel)
        {
            bool result = false;

            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            Brand? deletedBrand = await this.dbContext
                .Brands
                .FindAsync(inputModel.Id);

            if ((user != null) && (deletedBrand != null))
            {
                deletedBrand.IsDeleted = true;  

                await this.dbContext.SaveChangesAsync();

                result = true;
            }

            return result;
        }
    }
}
