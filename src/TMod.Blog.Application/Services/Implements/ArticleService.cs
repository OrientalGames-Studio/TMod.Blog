using MapsterMapper;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Article;
using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces.Services;
using TMod.Blog.Domain.Interfaces.UnitOfWorks;
using TMod.Blog.Infrastructure.Specifications;

namespace TMod.Blog.Application.Services.Implements
{
    internal class ArticleService : IArticleService
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;
        private readonly ILogger<ArticleService> _logger;
        private readonly ISlugService _slugService;
        private readonly IMapper _mapper;

        public ArticleService(IApplicationUnitOfWork applicationUnitOfWork, ILogger<ArticleService> logger, ISlugService slugService, IMapper mapper)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
            _logger = logger;
            _slugService = slugService;
            _mapper = mapper;
        }

        public async Task<ArticleDto> CreateArticleAsync(CreateArticleRequest createArticleRequest, CancellationToken cancellationToken = default)
        {
            Category? category = await _applicationUnitOfWork.CategoryRepository.GetEntityByIdAsync(createArticleRequest.CategoryId.GetValueOrDefault(),true,cancellationToken);
            if(category is null )
            {
                _logger.LogError("尝试把文章添加到不存在的分类[{}]", createArticleRequest.CategoryId);
                throw new NotSupportedException($"分类[{createArticleRequest.CategoryId}]不存在，不允许添加文章");
            }
            string slug = await _slugService.GenerateSlugAsync(createArticleRequest.Title,cancellationToken:cancellationToken);
            Article article = _mapper.Map<Article>(createArticleRequest);
            article.Slug = slug;
            await _applicationUnitOfWork.BeginTransactionAsync(cancellationToken);
            foreach ( string tagName in createArticleRequest.Tags )
            {
                Tag? tag = await _applicationUnitOfWork.TagRepository.GetByNameAsync(tagName,cancellationToken);
                if(tag is null )
                {
                    tag = new Tag()
                    {
                        Id = Guid.NewGuid(),
                        Name = tagName,
                    };
                    await _applicationUnitOfWork.TagRepository.AddAsync(tag, cancellationToken);
                }
                article.Tags.Add(tag);
            }
            await _applicationUnitOfWork.ArticleRepository.AddAsync(article, cancellationToken);
            await _applicationUnitOfWork.SaveChangesAsync(cancellationToken);
            await _applicationUnitOfWork.CommitTransactionAsync(cancellationToken);
            return _mapper.Map<ArticleDto>(article);
        }

        public async Task<bool> DeleteArticleAsync(Guid articleId, CancellationToken cancellationToken = default)
        {
            Article? meta = await _applicationUnitOfWork.ArticleRepository.GetEntityByIdAsync(articleId,true,cancellationToken);
            if(meta is null )
            {
                return false;
            }
            await _applicationUnitOfWork.BeginTransactionAsync(cancellationToken);
            _applicationUnitOfWork.ArticleRepository.Delete(meta);
            await _applicationUnitOfWork.SaveChangesAsync(cancellationToken);
            await _applicationUnitOfWork.CommitTransactionAsync(cancellationToken);
            return true;
        }

        public Task<ArticleDto?> GetArticleDetailAsync(Guid articleId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ArticleDto?> GetArticleDetailAsync(string slug, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagingDto<ArticleDto>> PagingArticleAsync(Guid? categoryId = null, Guid? tagId = null, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ArticleDto?> PatchArticleCategoryAsync(PatchArticleCategoryRequest patchArticleCategoryRequest, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ArticleDto?> PatchArticleIsCommentEnabledAsync(PatchArticleIsCommentEnabledRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ArticleDto?> PatchArticleIsShareEnabledAsync(PatchArticleIsShareEnabledRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ArticleDto?> PatchArticleTagsAsync(PatchArticleTagsRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ArticleDto?> UpdateArticleAsync(UpdateArticleRequest updateArticleRequest, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
