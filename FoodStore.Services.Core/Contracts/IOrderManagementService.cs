using FoodStore.ViewModels;
using FoodStore.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Services.Core.Contracts
{
    public interface IOrderManagementService
    {
        Task<PaginatedList<OrderAdminViewModel>> GetAllProcessedAndDeliveredOrdersAsync(string? status, int pageIndex, int pageSize);
        Task<bool> MarkOrderAsDeliveredAsync(int orderId);

        Task<DeliverOrderViewModel?> GetOrderViewModelByIdAsync(int orderId);

        Task<List<OrderAdminViewModel>> GetOrdersByStatusAsync(string? status);
    }
}
