using Azure.Core;

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
            ArticleDto articleDto = _mapper.Map<ArticleDto>(createArticleRequest);
            Article article = _mapper.Map<Article>(articleDto);
            article.CategoryId = category.Id;
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
                } else if ( tag.IsDeleted )
                {
                    tag.IsDeleted = false;
                    tag.DeleteDate = null;
                    _applicationUnitOfWork.TagRepository.Update(tag);
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
            foreach ( var item in meta.Tags )
            {
                if(item.Articles.Count == 0 )
                {
                    _applicationUnitOfWork.TagRepository.Delete(item);
                }
            }
            await _applicationUnitOfWork.SaveChangesAsync(cancellationToken);
            await _applicationUnitOfWork.CommitTransactionAsync(cancellationToken);
            return true;
        }

        public async Task<ArticleDto?> GetArticleDetailAsync(Guid articleId, CancellationToken cancellationToken = default)
        {
            Article? article = await _applicationUnitOfWork.ArticleRepository.GetArticleWithDetailByIdAsync(articleId, cancellationToken);
            if(article is null || article.IsDeleted )
            {
                return null;
            }
            return _mapper.Map<ArticleDto>(article);
        }

        public async Task<ArticleDto?> GetArticleDetailAsync(string slug, CancellationToken cancellationToken = default)
        {
            Article? article = await _applicationUnitOfWork.ArticleRepository.GetArticleBySlugAsync(slug, cancellationToken);
            if(article is null || article.IsDeleted )
            {
                return null;
            }
            return _mapper.Map<ArticleDto>(article);
        }

        public async Task<PagingDto<ArticleDto>> PagingArticleAsync(Guid? categoryId = null, Guid? tagId = null, string? keyword = null, ArticleStatusEnum articleStatus = ArticleStatusEnum.Draft | ArticleStatusEnum.Published | ArticleStatusEnum.Unpublished, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            pageIndex = Math.Max(1, pageIndex);
            int skip = (pageIndex - 1) * pageSize;
            var specification = ArticleSpecification.CreatePagingWithFullFilter(skip,Math.Max(0,pageSize),categoryId,tagId,keyword,articleStatus,false);
            var prepareSpecification = ArticleSpecification.CreateCountForPreparePaging(categoryId,tagId,keyword,articleStatus,false);
            int totalCount = await _applicationUnitOfWork.ArticleRepository.CountAsync(prepareSpecification,cancellationToken);
            var articles = await _applicationUnitOfWork.ArticleRepository.GetAllEntitiesAsync(specification,cancellationToken);
            int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);
            pageCount = Math.Max(1,pageCount);
            var articleDtos = _mapper.Map<List<ArticleDto>>(articles);
            return new PagingDto<ArticleDto>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                PageCount = pageCount,
                DataCount = totalCount,
                Items = articleDtos,
            };
        }

        public async Task<ArticleDto?> PatchArticleCategoryAsync(Guid articleId, PatchArticleCategoryRequest patchArticleCategoryRequest, CancellationToken cancellationToken = default)
        {
            Article? article = await _applicationUnitOfWork.ArticleRepository.GetEntityByIdAsync(articleId,false,cancellationToken);
            Category? category = await _applicationUnitOfWork.CategoryRepository.GetEntityByIdAsync(patchArticleCategoryRequest.CategoryId,true,cancellationToken);
            if(article is null || article.IsDeleted)
            {
                return null;
            }
            if(category is null || category.IsDeleted )
            {
                _logger.LogError("尝试把文章添加到不存在的分类[{}]", patchArticleCategoryRequest.CategoryId);
                throw new NotSupportedException($"分类[{patchArticleCategoryRequest.CategoryId}]不存在，不允许添加文章");
            }
            article.CategoryId = patchArticleCategoryRequest.CategoryId;
            article.Category = category;
            _applicationUnitOfWork.ArticleRepository.Update(article);
            await _applicationUnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ArticleDto>(article);
        }

        public async Task<ArticleDto?> PatchArticleIsCommentEnabledAsync(Guid articleId, PatchArticleIsCommentEnabledRequest request, CancellationToken cancellationToken = default)
        {
            Article? article = await _applicationUnitOfWork.ArticleRepository.GetEntityByIdAsync(articleId,false,cancellationToken);
            if(article is null || article.IsDeleted)
            {
                return null;
            }
            article.IsCommentEnabled = request.IsCommentEnabled;
            _applicationUnitOfWork.ArticleRepository.Update(article);
            await _applicationUnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ArticleDto>(article);
        }

        public async Task<ArticleDto?> PatchArticleIsShareEnabledAsync(Guid articleId, PatchArticleIsShareEnabledRequest request, CancellationToken cancellationToken = default)
        {
            Article? article = await _applicationUnitOfWork.ArticleRepository.GetEntityByIdAsync(articleId,false,cancellationToken);
            if ( article is null || article.IsDeleted )
            {
                return null;
            }
            article.IsShareEnabled = request.IsShareEnabled;
            _applicationUnitOfWork.ArticleRepository.Update(article);
            await _applicationUnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ArticleDto>(article);
        }

        public async Task<ArticleDto?> PatchArticleTagsAsync(Guid articleId, PatchArticleTagsRequest request, CancellationToken cancellationToken = default)
        {
            Article? article = await _applicationUnitOfWork.ArticleRepository.GetArticleWithDetailByIdAsync(articleId,cancellationToken);
            if ( article is null || article.IsDeleted )
            {
                return null;
            }
            await _applicationUnitOfWork.BeginTransactionAsync(cancellationToken);
            if ( request.Tags is not null && request.Tags.Count > 0 )
            {
                article.Tags.Clear();
                _applicationUnitOfWork.ArticleRepository.Update(article);
                foreach ( string tagName in request.Tags )
                {
                    Tag? tag = await _applicationUnitOfWork.TagRepository.GetByNameWithDetailAsync(tagName,cancellationToken);
                    if ( tag is null )
                    {
                        tag = new Tag()
                        {
                            Id = Guid.NewGuid(),
                            Name = tagName,
                        };
                        await _applicationUnitOfWork.TagRepository.AddAsync(tag, cancellationToken);
                    }
                    else if ( tag.IsDeleted )
                    {
                        tag.IsDeleted = false;
                        tag.DeleteDate = null;
                        _applicationUnitOfWork.TagRepository.Update(tag);
                    }
                    article.Tags.Add(tag);
                }
                _applicationUnitOfWork.ArticleRepository.Update(article);
            }
            else
            {
                if( request.RemovedTags is not null && request.RemovedTags.Count > 0 )
                {
                    foreach ( string tagName in request.RemovedTags )
                    {
                        Tag? tag = await _applicationUnitOfWork.TagRepository.GetByNameWithDetailAsync(tagName,cancellationToken);
                        if (tag is null)
                        {
                            continue;
                        }
                        article.Tags.Remove(tag);
                        tag.Articles.Remove(article);
                        if(tag.Articles.Count == 0 )
                        {
                            _applicationUnitOfWork.TagRepository.Delete(tag);
                        }
                    }
                }
                if(request.AddedTags is not null && request.AddedTags.Count > 0 )
                {
                    foreach ( string tagName in request.AddedTags )
                    {
                        Tag? tag = await _applicationUnitOfWork.TagRepository.GetByNameWithDetailAsync(tagName,cancellationToken);
                        if ( tag is null )
                        {
                            tag = new Tag()
                            {
                                Id = Guid.NewGuid(),
                                Name = tagName,
                            };
                            await _applicationUnitOfWork.TagRepository.AddAsync(tag, cancellationToken);
                        }
                        else if ( tag.IsDeleted )
                        {
                            tag.IsDeleted = false;
                            tag.DeleteDate = null;
                            _applicationUnitOfWork.TagRepository.Update(tag);
                        }
                        article.Tags.Add(tag);
                    }
                }
                _applicationUnitOfWork.ArticleRepository.Update(article);
            }
            await _applicationUnitOfWork.SaveChangesAsync(cancellationToken);
            await _applicationUnitOfWork.CommitTransactionAsync(cancellationToken);
            return _mapper.Map<ArticleDto>(article);
        }

        public async Task<ArticleDto?> UpdateArticleAsync(Guid articleId, UpdateArticleRequest updateArticleRequest, CancellationToken cancellationToken = default)
        {
            Article? article = await _applicationUnitOfWork.ArticleRepository.GetArticleWithDetailByIdAsync(articleId,cancellationToken);
            if ( article is null || article.IsDeleted )
            {
                return null;
            }
            await _applicationUnitOfWork.BeginTransactionAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(updateArticleRequest.Title) && !string.Equals(article.Title, updateArticleRequest.Title, StringComparison.InvariantCulture) )
            {
                article.Title = updateArticleRequest.Title;
            }
            if(updateArticleRequest.CategoryId is not null && updateArticleRequest.CategoryId != Guid.Empty && updateArticleRequest.CategoryId != article.CategoryId )
            {
                Category? category = await _applicationUnitOfWork.CategoryRepository.GetEntityByIdAsync(updateArticleRequest.CategoryId.GetValueOrDefault(),true,cancellationToken);
                if(category is null )
                {
                    _logger.LogError("尝试把文章添加到不存在的分类[{}]", updateArticleRequest.CategoryId);
                    throw new NotSupportedException($"分类[{updateArticleRequest.CategoryId}]不存在，不允许添加文章");
                }
                article.CategoryId = updateArticleRequest.CategoryId;
                article.Category = category;
            }
            if(updateArticleRequest.Tags is not null && updateArticleRequest.Tags.Count > 0 )
            {
                article.Tags.Clear();
                _applicationUnitOfWork.ArticleRepository.Update(article);
                foreach ( string tagName in updateArticleRequest.Tags )
                {
                    Tag? tag = await _applicationUnitOfWork.TagRepository.GetByNameAsync(tagName,cancellationToken);
                    if ( tag is null )
                    {
                        tag = new Tag()
                        {
                            Id = Guid.NewGuid(),
                            Name = tagName,
                        };
                        await _applicationUnitOfWork.TagRepository.AddAsync(tag, cancellationToken);
                    }
                    else if ( tag.IsDeleted )
                    {
                        tag.IsDeleted = false;
                        tag.DeleteDate = null;
                        _applicationUnitOfWork.TagRepository.Update(tag);
                    }
                    //if ( tag.Articles.Contains(article) )
                    //{
                    //    tag.Articles.Remove(article);
                    //    _applicationUnitOfWork.TagRepository.Update(tag);
                    //}
                    article.Tags.Add(tag);
                }
                _applicationUnitOfWork.ArticleRepository.Update(article);
            }
            article.Content = updateArticleRequest.Content ?? "";
            article.IsCommentEnabled = updateArticleRequest.IsCommentEnabled;
            article.IsShareEnabled = updateArticleRequest.IsShareEnabled;
            _applicationUnitOfWork.ArticleRepository.Update(article);
            await _applicationUnitOfWork.SaveChangesAsync(cancellationToken);
            await _applicationUnitOfWork.CommitTransactionAsync(cancellationToken);
            return _mapper.Map<ArticleDto>(article);
        }
    }
}
