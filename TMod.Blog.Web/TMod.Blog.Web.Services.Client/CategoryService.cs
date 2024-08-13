using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Categories;
using TMod.Blog.Web.Interactive.Abstraction;
using TMod.Blog.Web.Models;
using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Services.Client
{
    internal class CategoryService : ICategoryService
    {
        private readonly ICategoryApi _categoryApi;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryApi categoryApi, ILogger<CategoryService> logger)
        {
            _categoryApi = categoryApi;
            _logger = logger;
        }

        public Task<CategoryViewModel?> CreateCategoryAsync(string category)=>_categoryApi.CreateCategoryAsync(category);

        public Task<PagingResult<CategoryViewModel?>> GetAllCategoriesAsync(int pageSize, int pageIndex = 1, string? categoryKeyFilter = null, DateOnly? createDateFrom = null, DateOnly? createDateTo = null)=>_categoryApi.GetAllCategoriesAsync(pageSize,pageIndex,categoryKeyFilter,createDateFrom,createDateTo);

        public Task<CategoryViewModel?> GetCategoryByCategoryNameAsync(string? categoryName) => _categoryApi.GetCategoryByCategoryNameAsync(categoryName);

        public Task<CategoryViewModel?> SaveCategoryAsync(string? originCategory, string category)=>_categoryApi.SaveCategoryAsync(originCategory,category);

        public Task<CategoryViewModel?> UpdateCategoryAsync(string originCategory, string category) => _categoryApi.UpdateCategoryAsync(originCategory,category);

        public Task BatchRemoveCategoryByIdAsync(params int[] categoryIds) => _categoryApi.BatchRemoveCategoryByIdAsync(categoryIds);
    }
}
