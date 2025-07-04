using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

namespace FoodStore.Services.Core
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AdminService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;     
        }
        public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
        {
            var users = await userManager.Users.ToListAsync();

            var result = new List<UserViewModel>();

            foreach (var user in users)
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

            return result;
        }
    }
}
