using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.StoreServices;
using TMod.Blog.Data.Models.ViewModels.Categories;

namespace TMod.Blog.Data.Services
{
    public interface ICategoryStoreService:IStoreService,IPagingStoreService<CategoryViewModel>
    {
        IAsyncEnumerable<CategoryViewModel> GetAllCategoriesAsync();

        Task<CategoryViewModel?> LoadCategoryAsync(int id);

        Task<CategoryViewModel?> LoadCategoryAsync(string category);

        Task<CategoryViewModel> CreateCategoryAsync(string category);

        Task<CategoryViewModel?> UpdateCategoryAsync(int id,string category);

        Task<CategoryViewModel?> UpdateCategoryAsync(string originCategory,string category);

        Task RemoveCategoryAsync(int id);

        Task RemoveCategoryAsync(string category);

        Task BatchRemoveCategoryByIdAsync(params int[] categoryIds);

        Task<Guid> AppendCategoriesToArticleAsync(Guid articleId, IEnumerable<string> categories);

        Task SubstractCategoriesFromArticleAsync(Guid articleId, IEnumerable<string> categories);
    }
}
