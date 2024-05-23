using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.Repositories;
using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Repositories.Implements
{
    public abstract class BaseRepository<TKey, TModel> : IRepository<TKey, TModel> where TModel : class,IKey<TKey>
    {
        private readonly BlogContext _blogContext;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<TModel> _logger;
        protected BlogContext BlogContext => _blogContext;
        protected ILogger<TModel> Logger => _logger;

        public BaseRepository(BlogContext blogContext,ILoggerFactory loggerFactory)
        {
            _blogContext = blogContext;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<TModel>();
        }

        public virtual async Task<TModel> CreateAsync(TModel model)
        {
            try
            {
                EntityEntry<TModel> entityEntry = await BlogContext.Set<TModel>()
                    .AddAsync(model);
                int effectRows = await BlogContext.SaveChangesAsync();
                if ( effectRows > 0 )
                {
                    model = entityEntry.Entity;
                }
                else
                {
                    _logger.LogWarning($"向 {typeof(TModel)} 新增数据失败");
                }
            }
            catch ( Exception ex)
            {
                _logger.LogError(ex, $"向 {typeof(TModel)} 新增数据时发生异常");
            }
            return model;
        }

        public virtual Task<IEnumerable<TModel>> GetAllAsync() => Task.FromResult(BlogContext.Set<TModel>().AsNoTracking().AsEnumerable());
        public virtual async Task<TModel?> LoadAsync(TKey key) => await BlogContext.Set<TModel>().AsNoTracking().FirstOrDefaultAsync(p=>p.Id!.Equals(key));
        public virtual async Task RemoveAsync(TModel model)
        {
            try
            {
                BlogContext.Set<TModel>()
                    .Remove(model);
                await BlogContext.SaveChangesAsync();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"从 {typeof(TModel)} 中移除数据发生异常");
            }
        }
        public virtual async Task RemoveAsync(TKey key)
        {
            try
            {
                TModel? model = await LoadAsync(key);
                if(model is not null )
                {
                    await RemoveAsync(model.Id);
                    await BlogContext.SaveChangesAsync();
                }
                else
                {
                    _logger.LogWarning($"从 {typeof(TModel)} 中删除编号是 {key} 的数据失败，元数据不存在");
                }
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"从 {typeof(TModel)} 中移除数据发生异常");
            }
        }
        public virtual async Task<TModel> UpdateAsync(TModel model)
        {
            try
            {
                EntityEntry<TModel> entityEntry = BlogContext.Set<TModel>()
                    .Update(model);
                int effectRow = await BlogContext.SaveChangesAsync();
                if( effectRow > 0 )
                {
                    model = entityEntry.Entity;
                }
                else
                {
                    _logger.LogWarning($"修改 {typeof(TModel)} 的 {model.Id} 数据失败");
                }
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex,$"修改 {typeof(TModel)} 的 {model.Id} 数据时发生异常");
            }
            return model;
        }
    }
}
