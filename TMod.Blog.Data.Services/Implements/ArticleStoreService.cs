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
    internal class ArticleStoreService : IArticleStoreService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IArticleArchiveRepository _articleArchiveRepository;
        private readonly IArticleCategoryRepository _articleCategoryRepository;
        private readonly IArticleCommentRepository _articleCommentRepository;
        private readonly IArticleContentRepository _articleContentRepository;
        private readonly IArticleTagRepository _articleTagRepository;
        private readonly ILogger<ArticleStoreService> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly BlogContext _blogContext;

        public ArticleStoreService(IArticleRepository articleRepository, IArticleArchiveRepository articleArchiveRepository, IArticleCategoryRepository articleCategoryRepository, IArticleCommentRepository articleCommentRepository, IArticleContentRepository articleContentRepository, IArticleTagRepository articleTagRepository, ILogger<ArticleStoreService> logger, ICategoryRepository categoryRepository, BlogContext blogContext)
        {
            _articleRepository = articleRepository;
            _articleArchiveRepository = articleArchiveRepository;
            _articleCategoryRepository = articleCategoryRepository;
            _articleCommentRepository = articleCommentRepository;
            _articleContentRepository = articleContentRepository;
            _articleTagRepository = articleTagRepository;
            _logger = logger;
            _categoryRepository = categoryRepository;
            _blogContext = blogContext;
        }

        public ArticleArchiveContentViewModel GetArticleArchiveContent(Guid articleId, Guid archiveId)
        {
            Stream? fileDataStream = _articleArchiveRepository.GetArticleArchiveContent(articleId,archiveId,out string? fileName,out string? mimeType,out double? fileSize);
            ArticleArchiveContentViewModel viewModel = new ArticleArchiveContentViewModel()
            {
                ArchiveId = archiveId,
                ArchiveName = fileName??"",
                MIMEType = mimeType??"application/octet-stream",
                ArchiveData = fileDataStream,
                FileSize = fileSize.HasValue? fileSize.Value : 0d,
            };
            return viewModel;
        }

        public IEnumerable<ArticleArchiveViewModel> GetArticleArchivesByArticleId(Guid articleId)
        {
            IEnumerable<ArticleArchive?> archives = _articleArchiveRepository.GetArticleArchivesByArticleId(articleId);
            foreach ( ArticleArchive? archive in archives )
            {
                if(archive is null )
                {
                    continue;
                }
                yield return archive!;
            }
        }

        public async Task<ArticleViewModel?> GetArticleByIdAsync(Guid id)
        {
            Article? article = await _articleRepository.LoadAsync(id);
            if(article is null || article.IsRemove )
            {
                return null;
            }
            IEnumerable<ArticleCategory> categories = _articleCategoryRepository.GetArticleCategories(article.Id);
            IEnumerable<ArticleTag> tags = _articleTagRepository.GetArticleTags(article.Id);
            article.ArticleCategories = new List<ArticleCategory>(categories);
            article.ArticleTags = new List<ArticleTag>(tags);
            return article;
        }

        public ArticleContentViewModel GetArticleContentByArticleId(Guid articleId)
        {
            ArticleContent? content = _articleContentRepository.GetArticleContentByArticleId(articleId);
            if(content is null )
            {
                return new ArticleContentViewModel()
                {
                    ArticleId = articleId,
                };
            }
            return content!;
        }

        public async IAsyncEnumerable<ArticleViewModel?> GetArticlesAsync()
        {
            IEnumerable<Article> articles = await _articleRepository.GetAllAsync();
            articles = articles.Where(p => !p.IsRemove);
            foreach ( Article article in articles )
            {
                yield return article;
            }
        }

        public IQueryable<ArticleViewModel?> Paging(int pageIndex, int pageSize, out int totalDataCount, out int totalPageCount, Func<ArticleViewModel?, bool>? filter = null)
        {
            IEnumerable<ArticleViewModel?> articles = GetArticlesAsync().ToBlockingEnumerable();
            return Paging(articles, pageIndex, pageSize, out totalDataCount, out totalPageCount, filter);
        }

        public IQueryable<ArticleViewModel?> Paging(IEnumerable values, int pageIndex, int pageSize, out int totalDataCount, out int totalPageCount, Func<ArticleViewModel?, bool>? filter = null)
        {
            totalDataCount = 0;
            totalPageCount = 1;
            pageIndex = Math.Max(1, pageIndex);
            pageSize = Math.Max(1, pageSize);
            IEnumerable<ArticleViewModel?>? articles = values as IEnumerable<ArticleViewModel?>;
            if(articles is null )
            {
                articles = [];
            }
            if(filter is not null )
            {
                articles = articles.Where(filter);
            }
            totalDataCount = articles.Count();
            totalPageCount = Math.Max(1, ( int )Math.Ceiling(( double )totalDataCount / ( double )pageSize));
            pageIndex = Math.Max(1,Math.Min(pageIndex,totalPageCount));
            articles = articles.Skip(( pageIndex - 1 ) * pageSize).Take(pageSize);
            return articles.AsQueryable();
        }
    }
}
