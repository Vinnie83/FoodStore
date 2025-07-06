using FoodStore.Data;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
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

        public SupplierService(FoodStoreDbContext dbContext)
        {
            this.dbContext = dbContext;      
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
    }
}
