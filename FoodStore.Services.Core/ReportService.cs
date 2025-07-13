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

            var orderItems = await ordersQuery
                .SelectMany(o => o.Items.Select(oi => new
                {
                    Order = o,
                    Item = oi,
                    Product = oi.Product,
                    SupplierName = oi.Product.Supplier != null ? oi.Product.Supplier.Name : null,
                    BrandName = oi.Product.Brand != null ? oi.Product.Brand.Name : null,
                    CategoryName = oi.Product.Category != null ? oi.Product.Category.Name : null,
                    UserEmail = o.User != null ? o.User.Email : null
                }))
                .Where(x =>
                    filterType == null ||
                    (filterType == "user" && x.UserEmail != null && x.UserEmail.ToLower() == filterValue) ||
                    (filterType == "supplier" && x.SupplierName != null && x.SupplierName.ToLower() == filterValue) ||
                    (filterType == "brand" && x.BrandName != null && x.BrandName.ToLower() == filterValue) ||
                    (filterType == "category" && x.CategoryName != null && x.CategoryName.ToLower() == filterValue)
                )
                .ToListAsync();


            orderItems = filter switch
            {
                "order_id" => orderItems.OrderBy(x => x.Order.Id).ToList(),
                "date_asc" => orderItems.OrderBy(x => x.Order.OrderDate).ToList(),
                "date_desc" => orderItems.OrderByDescending(x => x.Order.OrderDate).ToList(),
                "product" => orderItems.OrderBy(x => x.Item.Product.Name).ToList(),
                "category_sort" => orderItems.OrderBy(x => x.Item.Product.Category.Name).ToList(),
                "brand_sort" => orderItems.OrderBy(x => x.Item.Product.Brand.Name).ToList(),
                "supplier_sort" => orderItems.OrderBy(x => x.Item.Product.Supplier.Name).ToList(),
                _ => orderItems
            };

            var reportData = orderItems
                .Select(x => new OrderReportViewModel
                {
                    OrderId = x.Order.Id,
                    OrderDate = x.Order.OrderDate.ToString(CreatedOnFormat),
                    ProductName = x.Item.Product.Name,
                    Category = x.Item.Product.Category.Name,
                    Brand = x.Item.Product.Brand.Name,
                    Supplier = x.Item.Product.Supplier.Name,
                    Quantity = x.Item.Quantity,
                    UserEmail = x.Order.User.Email
                })
                .ToList();

            return new OrdersReportPageViewModel
            {
                Reports = reportData,
                UserEmails = await dbContext.Users.Select(u => u.Email).Distinct().ToListAsync(),
                Suppliers = await dbContext.Suppliers.Select(s => s.Name).Distinct().ToListAsync(),
                Brands = await dbContext.Brands.Select(b => b.Name).Distinct().ToListAsync(),
                Categories = await dbContext.Categories.Select(c => c.Name).Distinct().ToListAsync()
            };

        }
    
    }
}
