using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Categories;
using TMod.Blog.Web.Models;

namespace TMod.Blog.Web.Services.Abstraction
{
    public interface ICategoryService
    {
        Task<PagingResult<CategoryViewModel?>> GetAllCategoriesAsync(int pageSize, int pageIndex = 1, string? categoryKeyFilter = null, DateOnly? createDateFrom = null, DateOnly? createDateTo = null);

        Task<CategoryViewModel?> GetCategoryByCategoryNameAsync(string? categoryName);

        Task<CategoryViewModel?> SaveCategoryAsync(string? originCategory, string category);

        Task<CategoryViewModel?> CreateCategoryAsync(string category);

        Task<CategoryViewModel?> UpdateCategoryAsync(string originCategory, string category);

        Task BatchRemoveCategoryByIdAsync(params int[] categoryIds);
    }
}
