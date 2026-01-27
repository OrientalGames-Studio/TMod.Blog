using MapsterMapper;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Comment;
using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Domain.Interfaces.UnitOfWorks;
using TMod.Blog.Infrastructure.Specifications;

namespace TMod.Blog.Application.Services.Implements
{
    internal class CommentService : ICommentService
    {
        private readonly ILogger<CommentService> _logger;
        private readonly ICommentUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(ILogger<CommentService> logger, ICommentUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentRequest request,string authorIp, CancellationToken cancellationToken = default)
        {
            if ( request.ArticleId is null || request.ArticleId == Guid.Empty )
            {
                _logger.LogError("文章编号不允许为空！");
                throw new InvalidOperationException("文章编号不允许为空！");
            }
            Article? article = await _unitOfWork.ArticleRepository.GetEntityByIdAsync(request.ArticleId.Value,true,cancellationToken);
            if ( article is null || article.IsDeleted)
            {
                _logger.LogWarning("{}({})试图评论不存在的文章，文章编号:{}", request.AuthorName, request.AuthorEmail, request.ArticleId);
                throw new InvalidOperationException("不允许评论不存在的文章");
            }
            if ( request.ParentId is not null && request.ParentId != Guid.Empty )
            {
                Comment? parentComment = await _unitOfWork.CommentRepository.GetEntityByIdAsync(request.ParentId.Value,true,cancellationToken);
                if(parentComment is null )
                {
                    _logger.LogError("评论不存在，无法评论，评论ID:{}", request.ParentId);
                    throw new InvalidOperationException("评论不存在，无法评论。");
                }
            }
            CommentDto commentDto = _mapper.Map<CommentDto>(request);
            commentDto.AuthorIp = authorIp;
            Comment comment = _mapper.Map<Comment>(commentDto);
            await _unitOfWork.CommentRepository.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<Comment,CommentDto>(comment,commentDto);
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken = default)
        {
            Comment? comment = await _unitOfWork.CommentRepository.GetEntityByIdAsync(commentId,false,cancellationToken);
            if(comment is null )
            {
                return false;
            }
            _unitOfWork.CommentRepository.Delete(comment);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<CommentDto?> GetCommentByIdAsync(Guid commentId, CancellationToken cancellationToken = default)
        {
            Comment? comment = await _unitOfWork.CommentRepository.GetEntityByIdAsync(commentId,true,cancellationToken);
            if(comment is null )
            {
                return null;
            }
            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<PagingDto<CommentDto>> PagingCommentsByArticleAsync(Guid articleId, int pageIndex = 1, int pageSize = 20, SortRuleEnum sortRule = SortRuleEnum.Latest, CancellationToken cancellationToken = default)
        {
            pageIndex = Math.Max(1, pageIndex);
            int skip = (pageIndex - 1) * pageSize;
            Article? article = await _unitOfWork.ArticleRepository.GetEntityByIdAsync(articleId,true,cancellationToken);
            if(article is null)
            {
                return new PagingDto<CommentDto>()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    DataCount = 0,
                    PageCount = 1,
                    Items = Array.Empty<CommentDto>()
                };
            }
            var countSpecification = CommentSpecification.CreateCountPagingByArticle(articleId);
            int totalCount = await _unitOfWork.CommentRepository.CountAsync(countSpecification,cancellationToken);
            var specification = CommentSpecification.CreatePagingArticleComment(articleId,skip,pageSize,sortRule);
            var comments = await _unitOfWork.CommentRepository.GetAllEntitiesAsync(specification,cancellationToken);
            int pageCount = (int)Math.Ceiling((double)totalCount / (double)pageSize);
            pageCount = Math.Max(1, pageCount);
            var commentDtos = _mapper.Map<List<CommentDto>>(comments)??[];
            return new PagingDto<CommentDto>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                PageCount = pageCount,
                DataCount = totalCount,
                Items = commentDtos
            };
        }

        public async Task<PagingDto<CommentDto>> PagingCommentsByCommentAsync(Guid commentId, int pageIndex = 1, int pageSize = 20, SortRuleEnum sortRule = SortRuleEnum.Latest, CancellationToken cancellationToken = default)
        {
            pageIndex = Math.Max(1, pageIndex);
            int skip = (pageIndex - 1) * pageSize;
            Comment? comment = await _unitOfWork.CommentRepository.GetEntityByIdAsync(commentId,true,cancellationToken);
            if(comment is null )
            {
                return new PagingDto<CommentDto>()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    DataCount = 0,
                    PageCount = 1,
                    Items = Array.Empty<CommentDto>()
                };
            }
            var countSpecification = CommentSpecification.CreateCountPagingByComment(commentId);
            int totalCount = await _unitOfWork.CommentRepository.CountAsync(countSpecification,cancellationToken);
            var specification = CommentSpecification.CreatePagingCommentReplies(commentId,skip,pageSize,sortRule);
            var comments = await _unitOfWork.CommentRepository.GetAllEntitiesAsync(specification,cancellationToken);
            int pageCount = (int)Math.Ceiling((double)totalCount / (double)pageSize);
            pageCount = Math.Max(1, pageCount);
            var commentDtos = _mapper.Map<List<CommentDto>>(comments)??[];
            return new PagingDto<CommentDto>()
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                PageSize = pageSize,
                DataCount = totalCount,
                Items = commentDtos
            };
        }
    }
}
