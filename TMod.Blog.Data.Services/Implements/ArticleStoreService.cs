using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TMod.Blog.Data.Models;
using TMod.Blog.Data.Models.DTO.Articles;
using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Data.Models.ViewModels.Categories;
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

        public async Task<bool> BatchUpdateArticleCommentEnabledFlagAsync(Dictionary<Guid, bool> dic)
        {
			using ( var trans = await _blogContext.Database.BeginTransactionAsync() )
			{
				try
				{
					foreach ( var item in dic )
					{
                        Article? article = await _articleRepository.LoadAsync(item.Key);
                        if ( article is null )
                        {
							_logger.LogWarning($"批量修改文章控评状态时，没有找到 id 为 {item.Key} 的文章，跳过该文章的修改。");
							continue;
                        }
                        if ( article.IsCommentEnabled == item.Value )
                        {
							_logger.LogWarning($"批量修改文章控评状态时，id 为 {item.Key} 的文章已是 {article.IsCommentEnabled} 状态，跳过该文章的修改。");
							continue;
                        }
                        article.UpdateMetaRecord(true);
                        article.IsCommentEnabled = item.Value;
                        article = await _articleRepository.UpdateAsync(article);
                    }
					await trans.CommitAsync();
					return true;
				}
				catch ( Exception ex )
				{
					_logger.LogError(ex, $"批量修改文章控评状态时发生异常，数据已尝试回滚。入参：({JsonSerializer.Serialize(dic)})");
					await trans.RollbackAsync();
					return false;
				}
			}
        }

        public async Task<bool> BatchUpdateArticleStateAsync(Dictionary<Guid, ArticleStateEnum> dic)
        {
			using ( var trans = await _blogContext.Database.BeginTransactionAsync() )
			{
				try
				{
					foreach ( var item in dic )
					{
						Article? article = await _articleRepository.LoadAsync(item.Key);
						if(article is null )
						{
							_logger.LogWarning($"批量修改文章状态时，没有找到 id 为 {item.Key} 的文章，跳过该文章的修改。");
							continue;
						}
						if(((short)item.Value & ((short)item.Value - 1)) != 0 )
						{
							_logger.LogWarning($"批量修改文章时，试图把 id 为 {item.Key} 的文章的状态修改为 {item.Value}，这不是一个有效的枚举值");
							continue;
						}
						if(article.State == (short)item.Value )
						{
							_logger.LogDebug($"批量修改文章时，id 为 {item.Key} 的状态已经是 {item.Value} 状态");
							continue;
						}
						article.UpdateMetaRecord(true);
						article.State = (short)item.Value;
						article = await _articleRepository.UpdateAsync(article);
					}
					await trans.CommitAsync();
					return true;
				}
				catch ( Exception ex )
				{
					_logger.LogError(ex, $"批量修改文章状态时发生异常，数据已尝试回滚。入参：{JsonSerializer.Serialize(dic)}");
					await trans.RollbackAsync();
					return false;
				}
			}
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
						Snapshot = string.IsNullOrWhiteSpace(articleModel.Snapshot)?articleModel.Content[..Math.Min(150,articleModel.Content.Length)]:articleModel.Snapshot,
						State = (short)articleModel.ArticleState,
						IsCommentEnabled = articleModel.IsCommentEnabled,
						LastEditDate = DateTime.Now
					};
					article.CreateMetaRecord();
					article = await _articleRepository.CreateAsync(article);
					// 文章正文
					ArticleContent content = new ArticleContent()
					{
						Article = article,
						Content = articleModel.Content
					};
					await _articleContentRepository.CreateAsync(content);
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
							string temp = $"{category.Category1[..Math.Min(4,category.Category1.Length)]}_{Guid.NewGuid():N}";
							temp = temp[..Math.Min(temp.Length, 20)];
							category.Category1 = temp;
							category.UpdateVersionRecord();
							_ = await _categoryRepository.UpdateAsync(category);
							category = new Category()
							{
								Category1 = item
							};
							category.CreateMetaRecord();
							category = await _categoryRepository.CreateAsync(category);
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
					await trans.CommitAsync();
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

        public async Task RemoveArticleByIdAsync(Guid articleId)
        {
			Article? article = await _articleRepository.LoadAsync(articleId);
			if(article is null || article.IsRemove )
			{
				return;
			}
			await _articleRepository.RemoveAsync(article);
        }

        public async Task<Guid> UpdateArticleAsync(Guid articleId
			, ArticleViewModel article
			, ArticleContentViewModel articleContent
			, IEnumerable<CategoryViewModel> deletedCategories
			, IEnumerable<CategoryViewModel> addedCategories
			, IEnumerable<ArticleTagViewModel> deletedTags
			, IEnumerable<ArticleTagViewModel> addedTags
			, IEnumerable<ArticleArchiveViewModel> deletedArchives
			, IEnumerable<AddArticleArchiveModel> addedArchives)
        {
			// 启用事务
			using ( var trans = await _blogContext.Database.BeginTransactionAsync() )
			{
				try
				{
					article.UpdateMetaRecord(true);
					// 写入主表数据
					article = (await _articleRepository.UpdateAsync(article!))!;
					// 写入正文数据
					articleContent = (await _articleContentRepository.UpdateAsync(articleContent!))!;
					// 删除移除的分类
					foreach ( CategoryViewModel category in deletedCategories )
					{
						ArticleCategory? articleCategory = _articleCategoryRepository.GetArticleCategoryById(articleId,category.Id);
						if ( articleCategory is not null )
						{
							await _articleCategoryRepository.RemoveAsync(articleCategory);
						}
					}
					// 增加新增的分类
					foreach ( CategoryViewModel category in addedCategories )
					{
						ArticleCategory articleCategory = new ArticleCategory()
						{
							Category = category!,
							Article = article!
						};
						await _articleCategoryRepository.CreateAsync(articleCategory);
					}
					// 删除移除的标签
					foreach ( ArticleTagViewModel tag in deletedTags )
					{
						await _articleTagRepository.RemoveAsync(tag!);
					}
					// 增加新增的标签
					foreach ( ArticleTagViewModel tag in addedTags )
					{
						await _articleTagRepository.CreateAsync(tag!);
					}
					// 删除移除的附件
					foreach ( ArticleArchiveViewModel archive in deletedArchives )
					{
						await _articleArchiveRepository.RemoveAsync(archive!);
					}
					// 增加新增的附件
					foreach ( AddArticleArchiveModel archive in addedArchives )
					{
						ArticleArchive articleArchive = new ArticleArchive()
						{
							Article = article!,
							ArchiveName = archive.FileName,
							ArchiveMimetype = archive.MIMEType,
							ArchiveFileSize = archive.FileSizeMB,
							ArchiveContent = await archive.ReadArchiveContentAsync()
						};
						await _articleArchiveRepository.CreateAsync(articleArchive);
					}
					trans.Commit();
				}catch(Exception ex )
				{
					_logger.LogError(ex, $"根据编号{articleId}修改文章时发生异常");
					trans.Rollback();
				}
			}
			return articleId;
        }

        public async Task<Guid> UpdateArticleCommentEnabledFlagAsync(Guid articleId, bool isEnabled)
        {
            Article? article = await _articleRepository.LoadAsync(articleId);
			if(article is null )
			{
				return Guid.Empty;
			}
			if(article.IsCommentEnabled == isEnabled )
			{
				return article.Id;
			}
			article.UpdateMetaRecord(true);
			article.IsCommentEnabled = isEnabled;
			article = await _articleRepository.UpdateAsync(article);
			return article.Id;
        }

        public async Task<Guid> UpdateArticleStateAsync(Guid articleId, ArticleStateEnum state)
        {
            Article? article = await _articleRepository.LoadAsync(articleId);
			if(article is null )
			{
				return Guid.Empty;
			}
			if(article.State == (short)state )
			{
				return article.Id;
			}
			article.UpdateMetaRecord(true);
			article.State = (short)state;
			switch ( state )
			{
				case ArticleStateEnum.Draft:
					article.PublishDate = null;
					break;
				case ArticleStateEnum.Published:
					article.PublishDate = DateTime.Now;
					break;
			}
			article = await _articleRepository.UpdateAsync(article);
			return article.Id;
		}
    }
}
