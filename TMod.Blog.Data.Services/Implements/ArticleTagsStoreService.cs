using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models;
using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Data.Repositories;

namespace TMod.Blog.Data.Services.Implements
{
    internal class ArticleTagsStoreService : IArticleTagsStoreService
    {
        private readonly ILogger<ArticleTagsStoreService> _logger;
        private readonly IArticleTagRepository _articleTagRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly BlogContext _blogContext;

        public ArticleTagsStoreService(ILogger<ArticleTagsStoreService> logger, IArticleTagRepository articleTagRepository, IArticleRepository articleRepository, BlogContext blogContext)
        {
            _logger = logger;
            _articleTagRepository = articleTagRepository;
            _articleRepository = articleRepository;
            _blogContext = blogContext;
        }

        public async Task<Guid> AddTagsToArticleAsync(Guid articleId, IEnumerable<string> tags)
        {
            Article? article = await _articleRepository.LoadAsync(articleId);
            if(article is null )
            {
                return Guid.Empty;
            }
            var metaTags = _articleTagRepository.GetArticleTags(articleId).Select(p=>p.Tag).ToArray();
            using ( var trans = await _blogContext.Database.BeginTransactionAsync() )
            {
                try
                {
                    foreach ( var tag in tags )
                    {
                        if(Array.IndexOf(metaTags,tag) > -1 )
                        {
                            continue;
                        }
                        ArticleTag articleTag = new ArticleTag()
                        {
                            ArticleId = article.Id,
                            Tag = tag,
                        };
                        await _articleTagRepository.CreateAsync(articleTag);
                    }
                    await trans.CommitAsync();
                    return article.Id;
                }
                catch ( Exception ex )
                {
                    _logger.LogError(ex, $"添加标签({string.Join(",",tags)})到文章{articleId}时发生异常");
                    await trans.RollbackAsync();
                    throw;
                }
            }
        }

        public IQueryable<ArticleTagViewModel?> Paging(int pageIndex, int pageSize, out int totalDataCount, out int totalPageCount, Func<ArticleTagViewModel?, bool>? filter = null)
        {
            var getAllTask = _articleTagRepository.GetAllAsync();
            getAllTask.Wait();
            IEnumerable<ArticleTagViewModel?> articleTags = from articleTag in getAllTask.Result
                                                            where ((ArticleTagViewModel?)articleTag) is not null
                                                            select ((ArticleTagViewModel?)articleTag)!;
            return Paging(articleTags,pageIndex,pageSize,out totalDataCount,out totalPageCount,filter);
        }

        public IQueryable<ArticleTagViewModel?> Paging(IEnumerable values, int pageIndex, int pageSize, out int totalDataCount, out int totalPageCount, Func<ArticleTagViewModel?, bool>? filter = null)
        {
            totalDataCount = 0;
            totalPageCount = 1;
            pageIndex = Math.Max(1, pageIndex);
            pageSize = Math.Max(1,pageSize);
            IEnumerable<ArticleTagViewModel?>? articleTags = values as IEnumerable<ArticleTagViewModel?>;
            if(articleTags is null )
            {
                articleTags = [];
            }
            if(filter is not null )
            {
                articleTags = articleTags.Where(filter);
            }
            totalDataCount = articleTags.Count();
            totalPageCount = Math.Max(1, (int)Math.Ceiling(( double )totalDataCount / ( double )pageSize));
            pageIndex = Math.Max(1, Math.Min(pageIndex, totalPageCount));
            articleTags = articleTags.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return articleTags.AsQueryable();
        }

        public async Task RemoveTagsFromArticleAsync(Guid articleId, IEnumerable<string> tags)
        {
            Article? article = await _articleRepository.LoadAsync(articleId);
            if ( article is null )
            {
                return;
            }
            using ( var trans = await _blogContext.Database.BeginTransactionAsync() )
            {
                try
                {
                    foreach ( var tag in tags )
                    {
                        ArticleTag? articleTag = _articleTagRepository.GetArticleTagByTag(articleId,tag);
                        if(articleTag is not null )
                        {
                            await _articleTagRepository.RemoveAsync(articleTag);
                        }
                    }
                    await trans.CommitAsync();
                }
                catch ( Exception ex )
                {
                    _logger.LogError(ex, $"从文章{articleId}中移除标签({string.Join(",", tags)})发生异常");
                    await trans.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
