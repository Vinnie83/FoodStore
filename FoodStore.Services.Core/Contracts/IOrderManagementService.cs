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
        Task<List<OrderAdminViewModel>> GetAllProcessedOrdersAsync();
        Task<bool> MarkOrderAsDeliveredAsync(int orderId);
    }
}
