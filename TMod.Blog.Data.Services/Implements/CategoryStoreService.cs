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
        private readonly BlogContext _context;

        public CategoryStoreService(ICategoryRepository categoryRepository, IArticleCategoryRepository articleCategoryRepository, IArticleRepository articleRepository, ILogger<CategoryStoreService> logger, BlogContext context)
        {
            _categoryRepository = categoryRepository;
            _articleCategoryRepository = articleCategoryRepository;
            _articleRepository = articleRepository;
            _logger = logger;
            _context = context;
        }

        public async Task<Guid> AppendCategoriesToArticleAsync(Guid articleId, IEnumerable<string> categories)
        {
            Article? article = await _articleRepository.LoadAsync(articleId);
            if ( article is null )
            {
                return Guid.Empty;
            }
            var tasks = from category in categories
                        select CreateCategoryAsync(category);
            await Task.WhenAll(tasks);
            IEnumerable<Category> metas = from category in tasks
                                          select ((Category)category.Result)!;
            using ( var trans = await _context.Database.BeginTransactionAsync() )
            {
                try
                {
                    foreach ( Category meta in metas )
                    {
                        ArticleCategory? relationships = _articleCategoryRepository.GetArticleCategoryById(article.Id,meta.Id);
                        if ( relationships is null )
                        {
                            ArticleCategory articleCategory = new ArticleCategory()
                            {
                                ArticleId = article.Id,
                                CategoryId = meta.Id
                            };
                            articleCategory = await _articleCategoryRepository.CreateAsync(articleCategory);
                        }
                    }
                    await trans.CommitAsync();
                    return article.Id;
                }
                catch ( Exception ex )
                {
                    _logger.LogError(ex, $"将分类({string.Join(",", categories)})添加文章{articleId}时发生异常");
                    await trans.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task BatchRemoveCategoryByIdAsync(params int[] categoryIds)
        {
            if(categoryIds is null || categoryIds.Length <= 0 )
            {
                return;
            }
            if(categoryIds.Length == 1 )
            {
                await RemoveCategoryAsync(categoryIds.First());
            }
            else
            {
                await _categoryRepository.BatchRemoveCategoryByIdAsync(categoryIds);
            }
        }

        public async Task<CategoryViewModel> CreateCategoryAsync(string category)
        {
            Category? meta = (await _categoryRepository.GetAllAsync()).FirstOrDefault(p=>p.Category1 == category);
            if ( meta is null )
            {
                meta = new Category();
                meta.Category1 = category;
                meta.CreateMetaRecord();
                meta = await _categoryRepository.CreateAsync(meta);
            }
            else if ( meta.IsRemove )
            {
                string temp = $"{meta.Category1[..Math.Min(4,meta.Category1.Length)]}_{Guid.NewGuid():N}";
                temp = temp[..Math.Min(temp.Length, 20)];
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
                if ( category?.IsRemove == true )
                {
                    return null;
                }
                if ( category is null )
                {
                    _logger.LogWarning($"没有找到编号为{category?.Id}的分类");
                }
                return category;
            }
            catch ( Exception ex )
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
                if ( meta is null )
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
            return Paging(categories, pageIndex, pageSize, out totalDataCount, out totalPageCount, filter);
        }

        public IQueryable<CategoryViewModel?> Paging(IEnumerable values, int pageIndex, int pageSize, out int totalDataCount, out int totalPageCount, Func<CategoryViewModel?, bool>? filter = null)
        {
            totalDataCount = 0;
            totalPageCount = 1;
            pageIndex = Math.Max(1, pageIndex);
            pageSize = Math.Max(1, pageSize);
            IEnumerable<CategoryViewModel>? categories = values as IEnumerable<CategoryViewModel>;
            if ( categories is null )
            {
                categories = values.Cast<CategoryViewModel>();
            }
            categories = categories.Where(p => !p.IsRemove);
            if ( filter is not null )
            {
                categories = categories.Where(p => filter.Invoke(p));
            }
            totalDataCount = categories.Count();
            totalPageCount = Math.Max(1, ( int )Math.Ceiling(( double )totalDataCount / ( double )pageSize));
            pageIndex = Math.Max(1, Math.Min(pageIndex, totalPageCount));
            categories = categories.Skip(( pageIndex - 1 ) * pageSize).Take(pageSize);
            List<CategoryViewModel?> viewModels = new List<CategoryViewModel?>();
            foreach ( Category? category in categories )
            {
                viewModels.Add(category);
            }
            return viewModels.AsQueryable();
        }

        public async Task RemoveCategoryAsync(int id)
        {
            await _categoryRepository.RemoveAsync(id);
        }

        public async Task RemoveCategoryAsync(string category)
        {
            Category? meta = (await _categoryRepository.GetAllAsync()).FirstOrDefault(p=>p.Category1 == category);
            if ( meta is not null )
            {
                await _categoryRepository.RemoveAsync(meta);
            }
        }

        public async Task SubstractCategoriesFromArticleAsync(Guid articleId, IEnumerable<string> categories)
        {
            Article? article = await _articleRepository.LoadAsync(articleId);
            if ( article is null )
            {
                return;
            }
            var tasks = from category in categories
                        select LoadCategoryAsync(category);
            await Task.WhenAll(tasks);
            IEnumerable<Category> metas = from task in tasks
                                           where task.Result is not null
                                           select ((Category)task.Result)!;
            using ( var trans = await _context.Database.BeginTransactionAsync() )
            {
                try
                {
                    foreach ( Category meta in metas )
                    {
                        ArticleCategory? relationship = _articleCategoryRepository.GetArticleCategoryById(articleId, meta.Id);
                        if(relationship is not null )
                        {
                            await _articleCategoryRepository.RemoveAsync(relationship);
                        }
                    }
                    await trans.CommitAsync();
                }
                catch ( Exception ex )
                {
                    _logger.LogError(ex, $"从文章{articleId}中移除分类({string.Join(",",categories)})发生异常");
                    await trans.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<CategoryViewModel?> UpdateCategoryAsync(int id, string category)
        {
            Category? meta = await _categoryRepository.LoadAsync(id);
            if ( meta?.IsRemove == true )
            {
                return null;
            }
            if ( meta is not null )
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
            if ( meta is not null )
            {
                meta.Category1 = category;
                meta.UpdateMetaRecord();
                meta = await _categoryRepository.UpdateAsync(meta);
            }
            return meta;
        }
    }
}
