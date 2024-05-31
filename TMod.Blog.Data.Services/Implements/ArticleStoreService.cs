﻿using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models;
using TMod.Blog.Data.Models.DTO.Articles;
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

        public async Task<Guid> CreateArticleAsync(AddArticleModel articleModel, IEnumerable<AddArticleArchiveModel> archiveModels)
        {
            using ( var trans = await _blogContext.Database.BeginTransactionAsync() )
            {
                try
                {
                    // 主表数据
                    Article article = new Article()
                    {
                        Title = articleModel.Title,
                        Snapshot = string.IsNullOrWhiteSpace(articleModel.Snapshot)?articleModel.Content.Substring(0,Math.Min(50,articleModel.Content.Length)):articleModel.Snapshot,
                        State = (short)articleModel.ArticleState,
                        IsCommentEnabled = articleModel.IsCommentEnabled,
                        LastEditDate = DateTime.Now
                    };
                    article.CreateMetaRecord();
                    article.UpdateMetaRecord();
                    article = await _articleRepository.CreateAsync(article);
                    // 文章分类
                    foreach ( var item in articleModel.Categories )
                    {
                        Category? category = _categoryRepository.GetCategoryByCategoryName(item);
                        if ( category is null )
                        {
                            category = new Category()
                            {
                                Category1 = item
                            };
                            category.CreateMetaRecord();
                            category = await _categoryRepository.CreateAsync(category);
                        }
                        else if ( category.IsRemove )
                        {
                            category.IsRemove = false;
                            category.RemoveDate = null;
                            category.UpdateMetaRecord(true);
                            category = await _categoryRepository.UpdateAsync(category);
                        }
                        ArticleCategory articleCategory = new ArticleCategory()
                        {
                            Article = article,
                            Category = category
                        };
                        await _articleCategoryRepository.CreateAsync(articleCategory);
                    }
                    // 文章标签
                    foreach ( var item in articleModel.Tags )
                    {
                        ArticleTag tag = new ArticleTag()
                        {
                            Article = article,
                            Tag = item
                        };
                        await _articleTagRepository.CreateAsync(tag);
                    }
                    // 附件
                    if(archiveModels is not null && archiveModels.Any() )
                    {
                        foreach ( var item in archiveModels )
                        {
                            ArticleArchive archive = new ArticleArchive()
                            {
                                Article = article,
                                ArchiveName = item.FileName,
                                ArchiveFileSize = item.FileSizeMB,
                                ArchiveMimetype = item.MIMEType,
                                ArchiveContent = await item.ReadArchiveContentAsync(),
                            };
                            await _articleArchiveRepository.CreateAsync(archive);
                        }
                    }
                    return article.Id;
                }
                catch ( Exception ex )
                {
                    _logger.LogError(ex, $"创建文章时发生异常，数据已尝试回滚");
                    await trans.RollbackAsync();
                }
            }
            return Guid.Empty;
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
