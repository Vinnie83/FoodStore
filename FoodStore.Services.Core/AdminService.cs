using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace FoodStore.Services.Core
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AdminService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;     
        }
        public async Task<PaginatedList<UserViewModel>> GetAllUsersAsync(int pageIndex, int pageSize)
        {
            var query = userManager.Users.OrderBy(u => u.UserName); 
            var totalCount = await query.CountAsync();
            var usersPage = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<UserViewModel>();

            foreach (var user in usersPage)
            {
                var roles = await userManager.GetRolesAsync(user);

                result.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles.ToList()
                });
            }

            return new PaginatedList<UserViewModel>(result, totalCount, pageIndex, pageSize);
        }

    }
}
