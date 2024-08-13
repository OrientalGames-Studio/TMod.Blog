using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models;

namespace TMod.Blog.Data.Repositories.Implements
{
    internal class CategoryRepository : BaseIntKeyRepository<Category>, ICategoryRepository
    {
        private readonly ILogger<CategoryRepository> _logger;
        public CategoryRepository(BlogContext blogContext, ILoggerFactory loggerFactory) : base(blogContext, loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CategoryRepository>();
        }

        public async Task BatchRemoveCategoryByIdAsync(params int[] categoryIds)
        {
            using ( var trans = await base.BlogContext.Database.BeginTransactionAsync() )
            {
                try
                {
                    foreach ( int id in categoryIds )
                    {
                        Category? category = await base.LoadAsync(id);
                        if (category is null)
                        {
                            _logger.LogWarning($"无法获取到 Id 是 {id} 的分类");
                            continue;
                        }
                        await base.RemoveAsync(category);
                    }
                    await base.BlogContext.SaveChangesAsync();
                    await trans.CommitAsync();
                }
                catch ( Exception ex )
                {
                    trans.Rollback();
                    _logger.LogError(ex, $"根据 Id 批量删除分类时发生异常， Id:({string.Join(",", categoryIds ?? [])})");
                    throw;
                }
            }
        }

        public Category? GetCategoryByCategoryName(string category)
        {
            Category? meta = base.BlogContext.Categories.FirstOrDefault(p=>p.Category1 == category);
            return meta;
        }
    }
}
