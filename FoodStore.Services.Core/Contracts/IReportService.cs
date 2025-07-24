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
        Task<OrdersReportPageViewModel> GetOrderReportsAsync(string? filter, int pageIndex, int pageSize);
        Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int orderId);

        Task<List<OrderReportViewModel>> GetAllOrderReportsAsync(string? filter);
        Task<byte[]> ExportOrdersToExcelAsync(string? filter);

        Task<ProductReportsPageViewModel> GetProductReportsAsync(string? filter, int pageIndex, int pageSize);
        Task<byte[]> ExportProductsToExcelAsync(string? filter);
    }
}
