using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Web.Interactive.Abstraction;
using TMod.Blog.Web.Models;
using TMod.Blog.Web.Models.Articles;
using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Services.Client
{
    internal class ArticleService : IArticleService
    {
        private readonly IArticleApi _articleApi;
        private readonly ILogger<ArticleService> _logger;

        public ArticleService(IArticleApi articleApi,ILogger<ArticleService> logger)
        {
            _articleApi = articleApi;
            _logger = logger;
        }

        public Task<bool> BatchRemoveArticleAsync(List<Guid> articleIds) => _articleApi.BatchRemoveArticleAsync(articleIds);

        public Task<bool> BatchUpdateArticleCommentIsEnabledAsync(Dictionary<Guid, bool> dic) => _articleApi.BatchUpdateArticleCommentIsEnabledAsync(dic);

        public Task<bool> BatchUpdateArticleStateAsync(Dictionary<Guid, ArticleStateEnum> articleStates) => _articleApi.BatchUpdateArticleStateAsync(articleStates);

        public Task<PagingResult<ArticleViewModel?>> GetAllArticleByPaging(int pageSize, QueryArticleFilterModel? filterModel, int pageIndex = 1)=>_articleApi.GetAllArticleByPaging(pageSize, filterModel, pageIndex);

        public Task<ArticleViewModel?> LoadArticleAsync(Guid articleId) => _articleApi.LoadArticleAsync(articleId);

        public Task<ArticleContentViewModel> LoadArticleContentAsync(Guid articleId) => _articleApi.LoadArticleContentAsync(articleId);

        public Task<bool> RemoveArticleAsync(Guid articleId) => _articleApi.RemoveArticleAsync(articleId);

        public Task<ArticleViewModel?> UpdateArticleCommentIsEnabledAsync(Guid articleId, bool isEnabled)=>_articleApi.UpdateArticleCommentIsEnabledAsync(articleId, isEnabled);

        public Task<ArticleViewModel?> UpdateArticleStateAsync(Guid articleId, ArticleStateEnum articleState) => _articleApi.UpdateArticleStateAsync(articleId, articleState);

        public Task<PagingResult<ArticleCommentViewModel?>> GetArticleCommentByPaging(int pageSize, Guid articleId, Guid? parentId = null, int pageIndex = 1, bool showAll = false) => _articleApi.GetArticleCommentByPaging(pageSize, articleId, parentId, pageIndex, showAll);

        public Task CommitReplyAsync(Guid replyArticleId, string? replyContent, string? email, string? nickName, bool notifyWhenReply, Guid? replyCommentId = null) => _articleApi.CommitReplyAsync(replyArticleId, replyContent, email, nickName, notifyWhenReply, replyCommentId);
    }
}
