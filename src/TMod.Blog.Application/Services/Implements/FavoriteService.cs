using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Domain.Interfaces.UnitOfWorks;
using TMod.Blog.Infrastructure.Specifications;

namespace TMod.Blog.Application.Services.Implements
{
    internal class FavoriteService : IFavoriteService
    {
        private readonly ILogger<FavoriteService> _logger;
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;

        public FavoriteService(ILogger<FavoriteService> logger, IApplicationUnitOfWork applicationUnitOfWork)
        {
            _logger = logger;
            _applicationUnitOfWork = applicationUnitOfWork;
        }

        public async Task<int> CountArticleFavoritesAsync(Guid articleId, CancellationToken token = default)
        {
            Article? article = await _applicationUnitOfWork.ArticleRepository.GetEntityByIdAsync(articleId,true,token);
            if(article is null )
            {
                _logger.LogWarning("文章{}不存在，获取点赞数量失败", articleId);
                return 0;
            }
            int favoriteCount = await _applicationUnitOfWork.FavoriteRepository.CountFavoriteByIdAsync(articleId,FavoriteTypeEnum.Article,token);
            return favoriteCount;
        }

        public async Task<int> CountCommentFavoritesAsync(Guid commentId, CancellationToken token = default)
        {
            Comment? comment = await _applicationUnitOfWork.CommentRepository.GetEntityByIdAsync(commentId,true,token);
            if(comment is null )
            {
                _logger.LogWarning("评论{}不存在，获取点赞数量失败", commentId);
                return 0;
            }
            int favoriteCount = await _applicationUnitOfWork.FavoriteRepository.CountFavoriteByIdAsync(commentId,FavoriteTypeEnum.Comment,token);
            return favoriteCount;
        }

        public async Task<bool> DisfavoriteArticleAsync(Guid articleId, string fingerprint, string clientIp, CancellationToken token = default)
        {
            Article? article = await _applicationUnitOfWork.ArticleRepository.GetEntityByIdAsync(articleId,true,token);
            if ( article is null || article.IsDeleted )
            {
                _logger.LogError("无法给不存在的文章{}取消点赞", articleId);
                return false;
            }
            var specification = FavoriteSpecification.CreateDisfavorite(articleId,fingerprint,clientIp,FavoriteTypeEnum.Article);
            var favoriteHistory = await _applicationUnitOfWork.FavoriteRepository.GetEntityAsync(specification,token);
            if(favoriteHistory is null || favoriteHistory.IsDeleted )
            {
                return true;
            }
            _applicationUnitOfWork.FavoriteRepository.Delete(favoriteHistory);
            await _applicationUnitOfWork.SaveChangesAsync(token);
            return true;
        }

        public async Task FavoriteArticleAsync(Guid articleId, string fingerprint, string clientIp, CancellationToken token = default)
        {
            Article? article = await _applicationUnitOfWork.ArticleRepository.GetEntityByIdAsync(articleId,true,token);
            if(article is null || article.IsDeleted)
            {
                _logger.LogError("无法给不存在的文章{}点赞", articleId);
                return;
            }
            Favorite favorite = new Favorite()
            {
                Id = Guid.Empty,
                Fingerprint = fingerprint,
                ClientIp = clientIp,
                FavoriteType = FavoriteTypeEnum.Article,
                TargetId = article.Id,
            };
            await _applicationUnitOfWork.FavoriteRepository.AddAsync(favorite, token);
            await _applicationUnitOfWork.SaveChangesAsync();
        }

        public async Task FavoriteCommentAsync(Guid commentId, string fingerprint, string clientIp, CancellationToken token = default)
        {
            Comment? comment = await _applicationUnitOfWork.CommentRepository.GetEntityByIdAsync(commentId,true,token);
            if ( comment is null )
            {
                _logger.LogWarning("无法给不存在的评论{}点赞", commentId);
                return;
            }
            Favorite favorite = new Favorite()
            {
                Id = Guid.Empty,
                Fingerprint = fingerprint,
                ClientIp = clientIp,
                FavoriteType = FavoriteTypeEnum.Comment,
                TargetId = comment.Id,
            };
            await _applicationUnitOfWork.FavoriteRepository.AddAsync(favorite, token);
            await _applicationUnitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<string>> GetArticleFavoritedListAsync(Guid articleId, CancellationToken token = default)
        {
            Article? article = await _applicationUnitOfWork.ArticleRepository.GetEntityByIdAsync(articleId,true,token);
            if ( article is null )
            {
                return [];
            }
            return await _applicationUnitOfWork.FavoriteRepository.GetFavoriteListAsync(articleId, FavoriteTypeEnum.Article, token).ContinueWith(t =>
            {
                IReadOnlyList<Favorite> favorites = t.Result;
                return favorites.Select(p => p.ClientIp).ToList();
            });
        }

        public async Task<bool> GetArticleIsFavoritedAsync(Guid articleId, string fingerprint, string clientIp, CancellationToken token = default)
        {
            Article? article = await _applicationUnitOfWork.ArticleRepository.GetEntityByIdAsync(articleId,true,token);
            if(article is null || article.IsDeleted )
            {
                return false;
            }
            return await _applicationUnitOfWork.FavoriteRepository.GetTargetIsFavoirtedAsync(articleId, FavoriteTypeEnum.Article, fingerprint, clientIp, token);
        }

        public async Task<IReadOnlyList<string>> GetCommentFavoirtedListAsync(Guid commentId, CancellationToken token = default)
        {
            Comment? comment = await _applicationUnitOfWork.CommentRepository.GetEntityByIdAsync(commentId,true,token);
            if ( comment is null )
            {
                return [];
            }
            return await _applicationUnitOfWork.FavoriteRepository.GetFavoriteListAsync(commentId, FavoriteTypeEnum.Comment, token).ContinueWith(t =>
            {
                IReadOnlyList<Favorite> favorites = t.Result;
                return favorites.Select(p => p.ClientIp).ToList();
            });
        }

        public async Task<bool> GetCommentIsFavoritedAsync(Guid commentId, string fingerprint, string clientIp, CancellationToken token = default)
        {
            Comment? comment = await _applicationUnitOfWork.CommentRepository.GetEntityByIdAsync(commentId,true,token);
            if ( comment is null )
            {
                return false;
            }
            return await _applicationUnitOfWork.FavoriteRepository.GetTargetIsFavoirtedAsync(commentId, FavoriteTypeEnum.Comment, fingerprint, clientIp, token);
        }
    }
}
