using FoodStore.Data;
using FoodStore.Data.Models.Enums;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;

using static FoodStore.GCommon.ValidationConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Services.Core
{
    public class OrderManagementService : IOrderManagementService
    {
        private readonly FoodStoreDbContext dbContext;

        public OrderManagementService(FoodStoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<OrderAdminViewModel>> GetAllProcessedOrdersAsync()
        {
            var orders = await this.dbContext.Orders
           .Where(o => o.OrderStatus == OrderStatus.Processed)
           .OrderByDescending(o => o.OrderDate)
           .Select(o => new OrderAdminViewModel
           {
               OrderId = o.Id,
               UserEmail = o.User.Email,
               OrderDate = o.OrderDate.ToString(CreatedOnFormat),
               TotalAmount = o.TotalAmount,
               PaymentStatus = o.PaymentStatus.ToString(),
               OrderStatus = o.OrderStatus.ToString()
           })
           .ToListAsync();

            return orders;
        }

        public async Task<bool> MarkOrderAsDeliveredAsync(int orderId)
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || order.OrderStatus != OrderStatus.Processed)
                return false;

            order.OrderStatus = OrderStatus.Delivered;
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
