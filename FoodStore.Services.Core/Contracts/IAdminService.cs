using FoodStore.ViewModels;
using FoodStore.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Services.Core.Contracts
{
    public interface IAdminService
    {
        Task<PaginatedList<UserViewModel>> GetAllUsersAsync(int pageIndex, int pageSize);
    }
}
