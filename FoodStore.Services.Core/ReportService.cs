using FoodStore.Data.Models;
using FoodStore.Data;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Services.Core
{
    public class ReportService : IReportService
    {
        private readonly FoodStoreDbContext dbContext;

        public ReportService(FoodStoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<OrderReportViewModel>> GetOrderReportsAsync(string? filter)
        {
            var ordersQuery = dbContext.Orders
        .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Brand)
        .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Supplier)
        .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Category)
        .AsQueryable();

            var orderItems = await ordersQuery
                .SelectMany(o => o.Items.Select(oi => new
                {
                    Order = o,
                    Item = oi
                }))
                .ToListAsync();

            // Apply ordering in-memory because navigation properties cannot be translated by EF Core in OrderBy
            orderItems = filter switch
            {
                "order_id" => orderItems.OrderBy(o => o.Order.Id).ToList(),
                "date_asc" => orderItems.OrderBy(x => x.Order.OrderDate).ToList(),
                "date_desc" => orderItems.OrderByDescending(x => x.Order.OrderDate).ToList(),
                "product" => orderItems.OrderBy(x => x.Item.Product.Name).ToList(),
                "category" => orderItems.OrderBy(x => x.Item.Product.Category.Name).ToList(),
                "brand" => orderItems.OrderBy(x => x.Item.Product.Brand.Name).ToList(),
                "supplier" => orderItems.OrderBy(x => x.Item.Product.Supplier.Name).ToList(),
                _ => orderItems
            };

            return orderItems
                .Select(x => new OrderReportViewModel
                {
                    OrderId = x.Order.Id,
                    OrderDate = x.Order.OrderDate,
                    ProductName = x.Item.Product.Name,
                    Category = x.Item.Product.Category.Name,
                    Brand = x.Item.Product.Brand.Name,
                    Supplier = x.Item.Product.Supplier.Name,
                    Quantity = x.Item.Quantity
                });
        }
    }
}
