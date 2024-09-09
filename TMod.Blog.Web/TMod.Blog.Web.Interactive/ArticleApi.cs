using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Web.Interactive.Abstraction;
using TMod.Blog.Web.Models;
using TMod.Blog.Web.Models.Articles;

namespace TMod.Blog.Web.Interactive
{
    internal class ArticleApi : IArticleApi
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<ArticleApi> _logger;

        public ArticleApi(IHttpClientFactory httpClientFactory, ILogger<ArticleApi> logger)
        {
            _apiClient = httpClientFactory.CreateClient("apiClient");
            _logger = logger;
        }

        public async Task<bool> BatchRemoveArticleAsync(List<Guid> articleIds)
        {
            string apiUrl = $"api/v1/admin/articles";
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete,apiUrl);
                request.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    articleIds
                }), Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));
                HttpResponseMessage? response = await _apiClient.SendAsync(request);
                if(response is null )
                {
                    return false;
                }
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口批量删除文章时发生异常");
                return false;
            }
        }

        public async Task<bool> BatchUpdateArticleCommentIsEnabledAsync(Dictionary<Guid, bool> dic)
        {
            if(dic is null || dic.Count == 0 )
            {
                return true;
            }
            try
            {
                string apiUrl = "api/v1/admin/articles/comments/state";
                HttpResponseMessage? response = await _apiClient.PatchAsJsonAsync(apiUrl,dic);
                if(response is null )
                {
                    return false;
                }
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口批量修改文章是否允许评论状态时发生异常");
                return false;
            }
        }

        public async Task<bool> BatchUpdateArticleStateAsync(Dictionary<Guid, ArticleStateEnum> articleStates)
        {
            if(articleStates is null || articleStates.Count == 0 )
            {
                return true;
            }
            try
            {
                string apiUrl = "api/v1/admin/articles/state";
                HttpResponseMessage? response = await _apiClient.PatchAsJsonAsync(apiUrl,new
                {
                    articleStates
                });
                if(response is null )
                {
                    return false;
                }
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口批量修改文章状态时发生异常");
                return false;
            }
        }

        public async Task CommitReplyAsync(Guid replyArticleId, string? replyContent, string? email, string? nickName,bool notifyWhenReply, Guid? replyCommentId = null)
        {
            try
            {
                string apiUrl = $"api/v1/Articles/{replyArticleId}/Comments{((replyCommentId is null || !replyCommentId.HasValue)?"":$"/{replyCommentId.Value}")}";
                object param = new
                {
                    articleId = replyArticleId,
                    parentCommentId = replyCommentId,
                    email = email,
                    nickName = nickName,
                    notifyNewReply = notifyWhenReply,
                    comment = replyContent
                };
                HttpResponseMessage response = await _apiClient.PostAsJsonAsync(apiUrl, param);
                response.EnsureSuccessStatusCode();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex,$"请求接口发表评论时发生异常");
            }
        }

        public async Task<PagingResult<ArticleViewModel?>> GetAllArticleByPaging(int pageSize, QueryArticleFilterModel? filterModel, int pageIndex = 1)
        {
            try
            {
                StringBuilder apiUrlBuilder = new($"api/v1/admin/articles?pageIndex={pageIndex}&pageSize={pageSize}");
                if(filterModel is not null )
                {
                    AppendFilterToUrl(apiUrlBuilder, filterModel);
                }
                string apiUrl = apiUrlBuilder.ToString();
                PagingResult<ArticleViewModel?>? result = await _apiClient.GetFromJsonAsync<PagingResult<ArticleViewModel?>>(apiUrl);
                if(result is null )
                {
                    return PagingResult<ArticleViewModel?>.Empty;
                }
                return result;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口获取所有文章时发生异常");
                return PagingResult<ArticleViewModel?>.Empty;
            }
        }

        public async Task<PagingResult<ArticleCommentViewModel?>> GetArticleCommentByPaging(int pageSize, Guid articleId, Guid? parentId = null, int pageIndex = 1,bool showAll = false)
        {
            string apiUrl = $"api/v1/articles/{articleId}/comments{((parentId is not null && parentId.HasValue)?$"/{parentId.Value}":"")}?pageIndex={pageIndex}&pageSize={pageSize}&showAll={showAll}";
            try
            {
                PagingResult<ArticleCommentViewModel?>? result = await _apiClient.GetFromJsonAsync<PagingResult<ArticleCommentViewModel?>>(apiUrl);
                if (result is null )
                {
                    return PagingResult<ArticleCommentViewModel?>.Empty;
                }
                return result;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口加载文章 {articleId} 的{((parentId is not null && parentId.HasValue)?"子":"")}评论时发生异常");
                return PagingResult<ArticleCommentViewModel?>.Empty;
            }
        }

        public async Task<ArticleViewModel?> LoadArticleAsync(Guid articleId)
        {
            try
            {
                string apiUrl = $"api/v1/admin/articles/{articleId}";
                ArticleViewModel? result = await _apiClient.GetFromJsonAsync<ArticleViewModel>(apiUrl);
                return result;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口加载编号是{articleId}的文章时发生异常");
                return null;
            }
        }

        public async Task<ArticleContentViewModel> LoadArticleContentAsync(Guid articleId)
        {
            try
            {
                string apiUrl = $"api/v1/articles/{articleId}/content";
                ArticleContentViewModel? content = await _apiClient.GetFromJsonAsync<ArticleContentViewModel>(apiUrl);
                if(content is null )
                {
                    _logger.LogWarning($"请求接口加载 id 是 {articleId} 的文章正文时，接口返回数据为 null");
                    return new ArticleContentViewModel() { ArticleId = articleId };
                }
                return content;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口加载 id 是 {articleId} 的文章正文时发生异常");
                return new ArticleContentViewModel() { ArticleId = articleId };
            }
        }

        public async Task<bool> RemoveArticleAsync(Guid articleId)
        {
            string apiUrl = $"api/v1/admin/articles/{articleId}";
            try
            {
                HttpResponseMessage? response = await _apiClient.DeleteAsync(apiUrl);
                if(response is null )
                {
                    return false;
                }
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口删除 id 是 {articleId} 的文章时发生异常");
                return false;
            }
        }

        public async Task<ArticleViewModel?> UpdateArticleCommentIsEnabledAsync(Guid articleId, bool isEnabled)
        {
            try
            {
                string apiUrl = $"api/v1/admin/articles/{articleId}/comments/state";
                HttpResponseMessage? result = await _apiClient.PatchAsJsonAsync<bool>(apiUrl,isEnabled);
                result.EnsureSuccessStatusCode();
                ArticleViewModel? article = await result.Content.ReadFromJsonAsync<ArticleViewModel>();
                return article;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口修改文章是否允许评论时发生异常，文章编号:{articleId}");
                return null;
            }
        }

        public async Task<ArticleViewModel?> UpdateArticleStateAsync(Guid articleId, ArticleStateEnum articleState)
        {
            if(((short)articleState & ((short)articleState - 1)) != 0 )
            {
                _logger.LogWarning($"请求接口修改文章{articleId}的状态时，传入了一个无效的枚举值:{articleState}");
                return null;
            }
            string apiUrl = $"api/v1/admin/articles/{articleId}/state";
            try
            {
                HttpResponseMessage? response = await _apiClient.PatchAsJsonAsync(apiUrl,new
                {
                    state = articleState
                });
                response.EnsureSuccessStatusCode();
                Guid articleIdJson = await response.Content.ReadFromJsonAsync<Guid>();
                return await LoadArticleAsync(articleIdJson);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口修改文章{articleId}的状态时发生异常");
                return null;
            }
        }

        private void AppendFilterToUrl(StringBuilder apiUrlBuilder, QueryArticleFilterModel filterModel)
        {
            if ( !string.IsNullOrWhiteSpace(filterModel.ArticleTitleFilter) )
            {
                apiUrlBuilder.Append($"&articleTitleFilter={HttpUtility.UrlEncode(filterModel.ArticleTitleFilter)}");
            }
            if ( !string.IsNullOrWhiteSpace(filterModel.ArticleSnapshotFilter) )
            {
                apiUrlBuilder.Append($"&articleSnapshotFilter={HttpUtility.UrlEncode(filterModel.ArticleSnapshotFilter)}");
            }
            if ( filterModel.ArticleCategoryFilter is not null && filterModel.ArticleCategoryFilter.Any() )
            {
                string categories = HttpUtility.UrlEncode(string.Join(",",filterModel.ArticleCategoryFilter));
                apiUrlBuilder.Append($"&articleCategoryFilter={categories}");
            }
            if ( filterModel.ArticleTagFilter is not null && filterModel.ArticleTagFilter.Any() )
            {
                string tags = HttpUtility.UrlEncode(string.Join(",",filterModel.ArticleTagFilter));
                apiUrlBuilder.Append($"&articleTagsFilter={tags}");
            }
            string dateStr = string.Empty;
            if(filterModel.MinArticleCreateDate is not null && filterModel.MinArticleCreateDate.HasValue)
            {
                dateStr = HttpUtility.UrlEncode(filterModel.MinArticleCreateDate.Value.ToString("yyyy-MM-dd"));
                apiUrlBuilder.Append($"&articleCreateDateFilter={dateStr}");
            }
            if(filterModel.MaxArticleCreateDate is not null && filterModel.MinArticleCreateDate.HasValue )
            {
                dateStr = HttpUtility.UrlEncode(filterModel.MaxArticleCreateDate.Value.ToString("yyyy-MM-dd"));
                apiUrlBuilder.Append($"&articleCreateDateEndFilter={dateStr}");
            }
            if(filterModel.MinArticleLastEditDate is not null && filterModel.MinArticleLastEditDate.HasValue )
            {
                dateStr = HttpUtility.UrlEncode(filterModel.MinArticleLastEditDate.Value.ToString("yyyy-MM-dd"));
                apiUrlBuilder.Append($"&articleLastEditDateFilter={dateStr}");
            }
            if(filterModel.MaxArticleLastEditDate is not null && filterModel.MaxArticleLastEditDate.HasValue )
            {
                dateStr = HttpUtility.UrlEncode(filterModel.MaxArticleLastEditDate.Value.ToString("yyyy-MM-dd"));
                apiUrlBuilder.Append($"&articleLastEditDateEndFilter={dateStr}");
            }
            if ( filterModel.MinArticlePublishedDate is not null && filterModel.MinArticlePublishedDate.HasValue )
            {
                dateStr = HttpUtility.UrlEncode(filterModel.MinArticlePublishedDate.Value.ToString("yyyy-MM-dd"));
                apiUrlBuilder.Append($"&articlePublishedDateFilter={dateStr}");
            }
            if ( filterModel.MaxArticlePublishedDate is not null && filterModel.MaxArticlePublishedDate.HasValue )
            {
                dateStr = HttpUtility.UrlEncode(filterModel.MaxArticlePublishedDate.Value.ToString("yyyy-MM-dd"));
                apiUrlBuilder.Append($"&articlePublishedDateEndFilter={dateStr}");
            }
            if(filterModel.ArticleStateFilter is not null )
            {
                apiUrlBuilder.Append($"&articleStateFilter={filterModel.ArticleStateFilter}");
            }
            if(filterModel.ArticleCommentStateFilter is not null && filterModel.ArticleCommentStateFilter.HasValue )
            {
                apiUrlBuilder.Append($"&articleCommentStateFilter={filterModel.ArticleCommentStateFilter}");
            }
        }
    }
}
