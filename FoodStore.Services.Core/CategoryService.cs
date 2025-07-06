using FoodStore.Data;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Services.Core
{

    public class CategoryService : ICategoryService
    {
        private readonly FoodStoreDbContext dbContext;

        public CategoryService(FoodStoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<AddCategoryDropDownMenu>> GetCategoriesDropDownAsync()
        {
            IEnumerable<AddCategoryDropDownMenu> categoriesAsDropDown = await this.dbContext
                .Categories
                .AsNoTracking()
                .Select(c => new AddCategoryDropDownMenu()
                {
                    Id = c.Id,
                    CategoryName = c.Name,
                })
                .ToArrayAsync();

            return categoriesAsDropDown;
            
        }
    }
}
