using FoodStore.Data.Models;
using FoodStore.Data;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;

using Microsoft.EntityFrameworkCore;
using static FoodStore.GCommon.ValidationConstants;

namespace FoodStore.Services.Core
{
    public class ReportService : IReportService
    {
        private readonly FoodStoreDbContext dbContext;

        public ReportService(FoodStoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task<OrdersReportPageViewModel> GetOrderReportsAsync(string? filter)
        {
            var ordersQuery = dbContext.Orders
                .Include(o => o.User)
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

            string? filterType = null;
            string? filterValue = null;

            if (!string.IsNullOrEmpty(filter) && filter.Contains(":"))
            {
                var parts = filter.Split(':', 2);
                filterType = parts[0].Trim().ToLower();
                filterValue = parts[1].Trim().ToLower();
            }

            var ordersList = await ordersQuery
                .Where(o =>
                    filterType == null ||
                    (filterType == "user" && o.User != null && o.User.Email.ToLower() == filterValue) ||
                    (filterType == "supplier" && o.Items.Any(i => i.Product.Supplier != null && i.Product.Supplier.Name.ToLower() == filterValue)) ||
                    (filterType == "brand" && o.Items.Any(i => i.Product.Brand != null && i.Product.Brand.Name.ToLower() == filterValue)) ||
                    (filterType == "category" && o.Items.Any(i => i.Product.Category != null && i.Product.Category.Name.ToLower() == filterValue))
                )
                .Select(o => new
                {
                    Order = o,
                    TotalPrice = o.Items.Sum(i => i.Quantity * i.Product.Price)
                })
                .Where(x => x.TotalPrice > 0)
                .ToListAsync();

            ordersList = filter switch
            {
                "order_id" => ordersList.OrderBy(x => x.Order.Id).ToList(),
                "date_asc" => ordersList.OrderBy(x => x.Order.OrderDate).ToList(),
                "date_desc" => ordersList.OrderByDescending(x => x.Order.OrderDate).ToList(),
                _ => ordersList
            };

            var reportData = ordersList.Select(x => new OrderReportViewModel
            {
                OrderId = x.Order.Id,
                OrderDate = x.Order.OrderDate.ToString(CreatedOnFormat),
                UserEmail = x.Order.User.Email,
                TotalPrice = x.TotalPrice
            }).ToList();

            return new OrdersReportPageViewModel
            {
                Reports = reportData,
                UserEmails = await dbContext.Users.Select(u => u.Email).Distinct().ToListAsync(),
                Suppliers = await dbContext.Suppliers.Select(s => s.Name).Distinct().ToListAsync(),
                Brands = await dbContext.Brands.Select(b => b.Name).Distinct().ToListAsync(),
                Categories = await dbContext.Categories.Select(c => c.Name).Distinct().ToListAsync()
            };
        }

        public async Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int orderId)
        {
            var order = await dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Brand)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Supplier)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return null;

            var viewModel = new OrderDetailsViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate.ToString(CreatedOnFormat),
                UserEmail = order.User?.Email ?? "Unknown",
                TotalPrice = order.Items.Sum(i => i.Product.Price * i.Quantity),
                Items = order.Items.Select(i => new OrderItemViewModel
                {
                    ProductName = i.Product.Name,
                    Category = i.Product.Category.Name,
                    Brand = i.Product.Brand.Name,
                    Supplier = i.Product.Supplier.Name,
                    Quantity = i.Quantity,
                    Price = i.Product.Price
                }).ToList()
            };

            return viewModel;
        }


    }
}
