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
    public class SupplierService : ISupplierService
    {
        private readonly FoodStoreDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public SupplierService(FoodStoreDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;    
            this.userManager = userManager;
        }

        public async Task<IEnumerable<AddSupplierDropDownMenu>> GetSuppliersDropDownAsync()
        {
            IEnumerable<AddSupplierDropDownMenu> addSuppliersAsDropDown = await this.dbContext
                .Suppliers
                .AsNoTracking()
                .Select(s => new AddSupplierDropDownMenu()
                {
                    Id = s.Id,
                    SupplierName = s.Name,

                })
                .ToArrayAsync();

            return addSuppliersAsDropDown;
        }

        public async Task<IEnumerable<SupplierViewModel>> GetAllSuppliersAsync()
        {
            var allSuppliers = await this.dbContext
                .Suppliers
                .AsNoTracking()
                .Select(s => new SupplierViewModel()
                {
                    Name = s.Name,
                    Phone = s.Phone,
                    EmailAddress = s.EmailAddress
                    
                })
                .ToListAsync();

            return allSuppliers;
        }

        public async Task<bool> AddSupplierAsync(string userId, AddSupplierInputModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Phone) || (string.IsNullOrWhiteSpace(model.EmailAddress)))
                return false;

            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            bool exists = await dbContext.Suppliers
                .AnyAsync(b => b.Name.ToLower() == model.Name.ToLower());

            if (exists)
                return false;

            if (user == null)
            {
                return false;
            }

            var newSupplier = new Supplier
            {
                Name = model.Name,
                Phone = model.Phone,
                EmailAddress = model.EmailAddress
            };

            await dbContext.Suppliers.AddAsync(newSupplier);
            await dbContext.SaveChangesAsync();

            return true;


        }
    }
}
