using FoodStore.Data.Models;
using FoodStore.Data;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;


using Microsoft.EntityFrameworkCore;
using static FoodStore.GCommon.ValidationConstants;
using System.ComponentModel;
using System.Drawing;

using OfficeOpenXml;
using OfficeOpenXml.Style;
using FoodStore.Data.Models.Enums;


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
                .Where(o => o.OrderStatus != OrderStatus.Pending)
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
                .AsNoTracking()
                .AsQueryable();

            bool includePending = false;
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
                    o.Items.Any(i => i.Quantity > 0 && i.Product.Price > 0) &&
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
            .AsNoTracking()
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

        public async Task<byte[]> ExportOrdersToExcelAsync(string? filter)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


            var report = await GetOrderReportsAsync(filter);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Orders");

            worksheet.Cells[1, 1].Value = "Order ID";
            worksheet.Cells[1, 2].Value = "Date";
            worksheet.Cells[1, 3].Value = "User Email";
            worksheet.Cells[1, 4].Value = "Total Price (lv)";

            int row = 2;
            foreach (var order in report.Reports)
            {
                worksheet.Cells[row, 1].Value = order.OrderId;
                worksheet.Cells[row, 2].Value = order.OrderDate;
                worksheet.Cells[row, 3].Value = order.UserEmail;
                worksheet.Cells[row, 4].Value = order.TotalPrice;
                row++;
            }

            worksheet.Cells.AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }

        public async Task<ProductReportsPageViewModel> GetProductReportsAsync(string? filter)
        {
            var query = dbContext.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.OrderItems)
            .AsNoTracking()
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.ToLower();

                if (filter.StartsWith("category:"))
                {
                    var val = filter["category:".Length..].Trim();
                    query = query.Where(p => p.Category.Name.ToLower() == val);
                }
                else if (filter.StartsWith("brand:"))
                {
                    var val = filter["brand:".Length..].Trim();
                    query = query.Where(p => p.Brand.Name.ToLower() == val);
                }
                else if (filter.StartsWith("supplier:"))
                {
                    var val = filter["supplier:".Length..].Trim();
                    query = query.Where(p => p.Supplier.Name.ToLower() == val);
                }
            }

            var reportData = await query
                .Select(p => new ProductReportViewModel
                {
                    ProductName = p.Name,
                    Category = p.Category.Name,
                    Brand = p.Brand.Name,
                    Supplier = p.Supplier.Name,
                    Price = p.Price,
                    StockQuantity = p.Quantity,
                    TotalOrderedQuantity = p.OrderItems.Sum(oi => oi.Quantity)
                })
                .Where(p => p.TotalOrderedQuantity > 0)
                .ToListAsync();

            return new ProductReportsPageViewModel
            {
                Reports = reportData,
                Categories = await dbContext.Categories.Select(c => c.Name).Distinct().ToListAsync(),
                Brands = await dbContext.Brands.Select(b => b.Name).Distinct().ToListAsync(),
                Suppliers = await dbContext.Suppliers.Select(s => s.Name).Distinct().ToListAsync()
            };
        }

        public async Task<byte[]> ExportProductsToExcelAsync(string? filter)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            var report = await GetProductReportsAsync(filter);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Products");

            worksheet.Cells[1, 1].Value = "Product";
            worksheet.Cells[1, 2].Value = "Category";
            worksheet.Cells[1, 3].Value = "Brand";
            worksheet.Cells[1, 4].Value = "Supplier";
            worksheet.Cells[1, 5].Value = "Price (lv)";
            worksheet.Cells[1, 6].Value = "Stock Quantity";
            worksheet.Cells[1, 7].Value = "Total Quantity Sold";
            worksheet.Cells[1, 8].Value = "Total Revenue (lv)";

            int row = 2;
            foreach (var product in report.Reports)
            {
                worksheet.Cells[row, 1].Value = product.ProductName;
                worksheet.Cells[row, 2].Value = product.Category;
                worksheet.Cells[row, 3].Value = product.Brand;
                worksheet.Cells[row, 4].Value = product.Supplier;
                worksheet.Cells[row, 5].Value = product.Price;
                worksheet.Cells[row, 6].Value = product.StockQuantity;
                worksheet.Cells[row, 7].Value = product.TotalOrderedQuantity;
                worksheet.Cells[row, 8].Value = product.TotalRevenue;
                row++;
            }

            worksheet.Cells.AutoFitColumns();
            return await package.GetAsByteArrayAsync();
        }
    }
}
