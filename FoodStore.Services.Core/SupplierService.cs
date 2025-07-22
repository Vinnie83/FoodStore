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
using static FoodStore.GCommon.ValidationConstants;
using Supplier = FoodStore.Data.Models.Supplier;

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

        public async Task<PaginatedList<SupplierViewModel>> GetAllSuppliersAsync(int pageIndex, int pageSize)
        {
            var allSuppliers = this.dbContext
                .Suppliers
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .Select(s => new SupplierViewModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Phone = s.Phone,
                    EmailAddress = s.EmailAddress

                });

            return await PaginatedList<SupplierViewModel>.CreateAsync(allSuppliers, pageIndex, pageSize);
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

        public async Task<EditSupplierInputModel?> GetSupplierForEditingAsync(string userId, int? id)
        {
            EditSupplierInputModel? editModel = null;

            if (id != null)
            {
                Supplier? supplier = await this.dbContext
                    .Suppliers
                    .AsNoTracking()
                    .SingleOrDefaultAsync(b => b.Id == id);

                ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

                if (supplier != null && user != null)
                {
                    editModel = new EditSupplierInputModel()
                    {
                        Id = supplier.Id,
                        Name = supplier.Name,
                        Phone = supplier.Phone,
                        EmailAddress = supplier.EmailAddress
                        
                    };
                }
            }

            return editModel;
        }

        public async Task<bool> EditSupplierAsync(string userId, EditSupplierInputModel inputModel)
        {
            bool result = false;

            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            Supplier? updatedSupplier = await this.dbContext
                .Suppliers
                .FindAsync(inputModel.Id);

            if ((user != null) &&
                (updatedSupplier != null))
            {
                updatedSupplier.Name = inputModel.Name;
                updatedSupplier.Phone = inputModel.Phone;
                updatedSupplier.EmailAddress = inputModel.EmailAddress;

                await this.dbContext.SaveChangesAsync();

                result = true;
            }

            return result;
        }

        public async Task<SupplierDeleteViewModel?> GetSupplierForDeletingAsync(string userId, int? supplierId)
        {
            SupplierDeleteViewModel? deleteModel = null;

            if (supplierId != null)
            {
                Supplier? supplier = await this.dbContext
                .Suppliers
                .AsNoTracking()
                .SingleOrDefaultAsync(b => b.Id == supplierId);

                ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

                if ((user != null) && (supplier != null))
                {
                    deleteModel = new SupplierDeleteViewModel()
                    {
                        Id = supplier.Id,
                        Name = supplier .Name,
                        Phone = supplier .Phone,
                        EmailAddress = supplier .EmailAddress,
                    };

                }
            }

            return deleteModel;
        }

        public async Task<bool> SoftDeleteSupplierAsync(string userId, SupplierDeleteViewModel inputModel)
        {
            bool result = false;

            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            Supplier? deletedSupplier = await this.dbContext
                .Suppliers
                .FindAsync(inputModel.Id);

            if ((user != null) && (deletedSupplier != null))
            {
                deletedSupplier.IsDeleted = true;

                await this.dbContext.SaveChangesAsync();

                result = true;
            }

            return result;
        }
    }
}
