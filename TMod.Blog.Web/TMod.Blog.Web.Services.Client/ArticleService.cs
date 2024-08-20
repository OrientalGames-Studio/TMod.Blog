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

        public Task<bool> BatchUpdateArticleCommentIsEnabledAsync(Dictionary<Guid, bool> dic) => _articleApi.BatchUpdateArticleCommentIsEnabledAsync(dic);

        public Task<PagingResult<ArticleViewModel?>> GetAllArticleByPaging(int pageSize, QueryArticleFilterModel? filterModel, int pageIndex = 1)=>_articleApi.GetAllArticleByPaging(pageSize, filterModel, pageIndex);

        public Task<ArticleViewModel?> UpdateArticleCommentIsEnabledAsync(Guid articleId, bool isEnabled)=>_articleApi.UpdateArticleCommentIsEnabledAsync(articleId, isEnabled);
    }
}
