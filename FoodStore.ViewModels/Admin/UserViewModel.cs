using Microsoft.AspNetCore.Identity;

namespace FoodStore.ViewModels.Admin
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
