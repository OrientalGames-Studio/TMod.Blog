using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models;
using TMod.Blog.Data.Models.ViewModels.Categories;
using TMod.Blog.Data.Repositories;

namespace TMod.Blog.Data.Services.Implements
{
    internal class CategoryStoreService : ICategoryStoreService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IArticleCategoryRepository _articleCategoryRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly ILogger<CategoryStoreService> _logger;

        public CategoryStoreService(ICategoryRepository categoryRepository, IArticleCategoryRepository articleCategoryRepository, IArticleRepository articleRepository, ILogger<CategoryStoreService> logger)
        {
            _categoryRepository = categoryRepository;
            _articleCategoryRepository = articleCategoryRepository;
            _articleRepository = articleRepository;
            _logger = logger;
        }

        public async Task<CategoryViewModel> CreateCategoryAsync(string category)
        {
            Category? meta = (await _categoryRepository.GetAllAsync()).FirstOrDefault(p=>p.Category1 == category);
            if(meta is null )
            {
                meta = new Category();
                meta.Category1 = category;
                meta.CreateMetaRecord();
                meta = await _categoryRepository.CreateAsync(meta);
            } else if ( meta.IsRemove )
            {
                string temp = $"{meta.Category1[..Math.Min(4,meta.Category1.Length)]}_{Guid.NewGuid():N}";
                temp = temp[.. Math.Min(temp.Length, 20)];
                meta.Category1 = temp;
                meta.UpdateVersionRecord();
                _ = await _categoryRepository.UpdateAsync(meta);
                meta = new Category();
                meta.Category1 = category;
                meta.CreateMetaRecord();
                meta = await _categoryRepository.CreateAsync(meta);
            }
            return meta!;
        }

        public async IAsyncEnumerable<CategoryViewModel> GetAllCategoriesAsync()
        {
            IEnumerable<Category> categories = (await _categoryRepository.GetAllAsync()).Where(p=>!p.IsRemove);
            foreach ( var item in categories )
            {
                CategoryViewModel? vm = item;
                vm ??= new CategoryViewModel()
                {
                    Id = item.Id,
                    Category = item.Category1,
                    IsRemove = item.IsRemove,
                    Version = item.Version,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate,
                    RemoveDate = item.RemoveDate
                };
                yield return vm;
            }
        }

        public async Task<CategoryViewModel?> LoadCategoryAsync(int id)
        {
            try
            {
                Category? category = await _categoryRepository.LoadAsync(id);
                if(category?.IsRemove == true )
                {
                    return null;
                }
                if ( category is null )
                {
                    _logger.LogWarning($"没有找到编号为{category?.Id}的分类");
                }
                return category;
            }catch(Exception ex)
            {
                _logger.LogError(ex, $"加载编号为{id}的分类项发生异常");
                return null;
            }
        }

        public async Task<CategoryViewModel?> LoadCategoryAsync(string category)
        {
            try
            {
                Category? meta = (await _categoryRepository.GetAllAsync()).FirstOrDefault(p=>p.Category1 == category);
                if ( meta?.IsRemove == true )
                {
                    return null;
                }
                if (meta is null )
                {
                    _logger.LogWarning($"没有找到分类:{category}");
                }
                return meta;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"加载分类名称为{category}的分类项发生异常");
                return null;
            }
        }

        public IQueryable<CategoryViewModel?> Paging(int pageIndex, int pageSize, out int totalDataCount, out int totalPageCount, Func<CategoryViewModel?, bool>? filter = null)
        {
            Task<Task<IEnumerable<Category>>> getAllTask = new Task<Task<IEnumerable<Category>>>(async ()=> await _categoryRepository.GetAllAsync());
            getAllTask.RunSynchronously();
            IEnumerable<Category> categories = getAllTask.Result.Result;
            return Paging(categories,pageIndex, pageSize, out totalDataCount, out totalPageCount, filter);
        }

        public IQueryable<CategoryViewModel?> Paging(IEnumerable values, int pageIndex, int pageSize, out int totalDataCount, out int totalPageCount, Func<CategoryViewModel?, bool>? filter = null)
        {
            totalDataCount = 0;
            totalPageCount = 1;
            pageIndex = Math.Max(1, pageIndex);
            pageSize = Math.Max(1, pageSize);
            IEnumerable<Category>? categories = values as IEnumerable<Category>;
            if(categories is null )
            {
                categories = [];
            }
            categories = categories.Where(p => !p.IsRemove);
            if(filter is not null )
            {
                categories = categories.Where(p => filter.Invoke(p));
            }
            totalDataCount = categories.Count();
            totalPageCount = Math.Max(1, ( int )Math.Ceiling(( double )totalDataCount / ( double )pageSize));
            pageIndex = Math.Max(1, Math.Min(pageIndex, totalPageCount));
            categories = categories.Skip(( pageIndex - 1 ) * pageSize).Take(pageSize);
            List<CategoryViewModel?> viewModels = new List<CategoryViewModel?>();
            foreach ( Category category in categories )
            {
                viewModels.Add( category );
            }
            return viewModels.AsQueryable();
        }

        public async Task RemoveCategoryAsync(int id)
        {
            await _categoryRepository.RemoveAsync( id );
        }

        public async Task RemoveCategoryAsync(string category)
        {
            Category? meta = (await _categoryRepository.GetAllAsync()).FirstOrDefault(p=>p.Category1 == category);
            if(meta is not null )
            {
                await _categoryRepository.RemoveAsync(meta);
            }
        }

        public async Task<CategoryViewModel?> UpdateCategoryAsync(int id, string category)
        {
            Category? meta = await _categoryRepository.LoadAsync(id);
            if ( meta?.IsRemove == true )
            {
                return null;
            }
            if (meta is not null)
            {
                meta.Category1 = category;
                meta.UpdateMetaRecord(true);
                meta = await _categoryRepository.UpdateAsync(meta);
            }
            return meta;
        }

        public async Task<CategoryViewModel?> UpdateCategoryAsync(string originCategory, string category)
        {
            Category? meta = (await _categoryRepository.GetAllAsync()).FirstOrDefault(p=>p.Category1 == originCategory);
            if ( meta?.IsRemove == true )
            {
                return null;
            }
            if (meta is not null )
            {
                meta.Category1 = category;
                meta.UpdateMetaRecord();
                meta = await _categoryRepository.UpdateAsync(meta);
            }
            return meta;
        }
    }
}
