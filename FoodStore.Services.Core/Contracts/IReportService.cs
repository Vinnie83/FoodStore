using FoodStore.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Services.Core.Contracts
{
    public interface IReportService
    {
        Task<OrdersReportPageViewModel> GetOrderReportsAsync(string? filter);
        Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int orderId);

        Task<byte[]> ExportOrdersToExcelAsync(string? filter);

        Task<ProductReportsPageViewModel> GetProductReportsAsync(string? filter);
        Task<byte[]> ExportProductsToExcelAsync(string? filter);
    }
}
