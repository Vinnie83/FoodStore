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
        public async Task<List<OrderAdminViewModel>> GetAllProcessedAndDeliveredOrdersAsync()
        {
            var orders = await this.dbContext.Orders
           .Where(o => o.OrderStatus == OrderStatus.Processed || o.OrderStatus == OrderStatus.Delivered)
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

        public async Task<DeliverOrderViewModel?> GetOrderViewModelByIdAsync(int orderId)
        {
            var order = await dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId
                && o.OrderStatus == OrderStatus.Processed
                && o.PaymentStatus == PaymentStatus.Paid);

            if (order == null)
                return null;

            return new DeliverOrderViewModel
            {
                OrderId = order.Id,
                UserEmail = order.User?.Email ?? "Unknown",
                OrderDate = order.OrderDate.ToString(CreatedOnFormat),
                PaymentStatus = order.PaymentStatus.ToString(),
                TotalAmount = order.Items.Sum(i => i.Price * i.Quantity).ToString("F2") + " lv"
            };
        }

        public async Task<List<OrderAdminViewModel>> GetOrdersByStatusAsync(string? status)
        {
            var query = dbContext.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var parsedStatus))
            {
                query = query.Where(o => o.OrderStatus == parsedStatus);
            }

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderAdminViewModel
                {
                    OrderId = o.Id,
                    UserEmail = o.User.Email,
                    OrderDate = o.OrderDate.ToString("yyyy-MM-dd HH:mm"),
                    TotalAmount = o.TotalAmount,
                    PaymentStatus = o.PaymentStatus.ToString(),
                    OrderStatus = o.OrderStatus.ToString()
                })
                .ToListAsync();

            return orders;
        }
    }
}
