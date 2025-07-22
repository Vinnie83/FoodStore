using FoodStore.ViewModels;
using FoodStore.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Services.Core.Contracts
{
    public interface ISupplierService 
    {
        Task<IEnumerable<AddSupplierDropDownMenu>> GetSuppliersDropDownAsync();
        Task<PaginatedList<SupplierViewModel>> GetAllSuppliersAsync(int pageIndex, int pageSize);

        Task<bool> AddSupplierAsync(string userId, AddSupplierInputModel inputModel);

        Task<EditSupplierInputModel?> GetSupplierForEditingAsync(string userId, int? id);

        Task<bool> EditSupplierAsync(string userId, EditSupplierInputModel inputModel);

        Task<SupplierDeleteViewModel?> GetSupplierForDeletingAsync(string userId, int? supplierId);

        Task<bool> SoftDeleteSupplierAsync(string userId, SupplierDeleteViewModel inputModel);
    }
}
