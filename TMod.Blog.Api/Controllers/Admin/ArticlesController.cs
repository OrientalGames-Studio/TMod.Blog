using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Data.Services;

namespace TMod.Blog.Api.Controllers.Admin
{
    [Route("api/v1/admin/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ILogger<ArticlesController> _logger;
        private readonly IArticleStoreService _articleStoreService;

        public ArticlesController(ILogger<ArticlesController> logger, IArticleStoreService articleStoreService)
        {
            _logger = logger;
            _articleStoreService = articleStoreService;
        }

        /// <summary>
        /// 分页查询文章列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">单页数据量</param>
        /// <param name="articleTitleFilter">文章标题筛选</param>
        /// <param name="articlePublishedDateFilter">发布日期筛选</param>
        /// <param name="articleLastEditDateFilter">最后编辑日期筛选</param>
        /// <param name="articleStateFilter">文章状态筛选</param>
        /// <param name="articleSnapshotFilter">文章快照内容筛选</param>
        /// <param name="articleCategoryFilter">文章分类筛选</param>
        /// <param name="articleTagsFilter">文章标签筛选，这个通常有可能会有多个，多个时按( ,，;；)分隔</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllArticle([FromQuery] int pageIndex = 1    // 页码
            , [FromQuery] int pageSize = 20 // 单页数据量
            , [FromQuery] string? articleTitleFilter = null  // 文章标题筛选
            , [FromQuery] DateOnly? articlePublishedDateFilter = null // 发布日期筛选
            , [FromQuery] DateOnly? articleLastEditDateFilter = null   // 最后编辑日期筛选
            , [FromQuery] ArticleStateEnum? articleStateFilter = null   // 文章状态筛选
            , [FromQuery] string? articleSnapshotFilter = null  // 文章快照内容筛选
            , [FromQuery] string? articleCategoryFilter = null  // 文章分类筛选
            , [FromQuery] string? articleTagsFilter = null  // 文章标签筛选，这个通常有可能会有多个，多个时按( ,，;；)分隔
            )
        {
            object pagingResult;
            try
            {
                IQueryable<ArticleViewModel?> articles = _articleStoreService.Paging(pageIndex,pageSize,out int dataCount,out int pageCount,article=>FilterArticle(article,articleTitleFilter,articleSnapshotFilter,articleCategoryFilter,articleTagsFilter,articleStateFilter,articlePublishedDateFilter,articleLastEditDateFilter));
                pagingResult = new
                {
                    pageIndex,
                    pageSize,
                    dataCount,
                    pageCount,
                    data = articles
                };
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"分页查询文章时发生异常，当前页码:{pageIndex}，单页数据量:{pageSize}");
                pagingResult = new
                {
                    pageIndex,
                    pageSize,
                    dataCount = 0,
                    pageCount = 1,
                    data = new List<ArticleViewModel>()
                };
            }
            return Ok(pagingResult);
        }

        [ActionName(nameof(GetArticleByIdAsync))]
        [HttpGet("{id:guid}",Name = nameof(GetArticleByIdAsync))]
        public async Task<IActionResult> GetArticleByIdAsync([FromRoute]Guid id)
        {
            ArticleViewModel? viewModel = await _articleStoreService.GetArticleByIdAsync(id);
            if(viewModel is null )
            {
                return NotFound();
            }
            return Ok(viewModel);
        }

        private bool FilterArticle(ArticleViewModel? article, string? articleTitleFilter, string? articleSnapshotFilter, string? articleCategoryFilter, string? articleTagsFilter, ArticleStateEnum? articleStateFilter, DateOnly? articlePublishedDateFilter, DateOnly? articleLastEditDateFilter)
        {
            bool result = true;
            if ( article is null )
            {
                return false;
            }
            if ( !string.IsNullOrWhiteSpace(articleTitleFilter) )
            {
                result = result && article.Title.Contains(articleTitleFilter);
            }
            if ( !string.IsNullOrWhiteSpace(articleSnapshotFilter) )
            {
                result = result && ( article.Snapshot?.Contains(articleSnapshotFilter) == true );
            }
            if ( articlePublishedDateFilter is not null )
            {
                result = result && ( article.State == ArticleStateEnum.Published && DateOnly.FromDateTime(article.PublishDate!.Value) == articlePublishedDateFilter );
            }
            if ( articleLastEditDateFilter is not null && article.LastEditDate is not null )
            {
                result = result && ( DateOnly.FromDateTime(article.LastEditDate!.Value) == articleLastEditDateFilter );
            }
            if ( articleStateFilter is not null )
            {
                result = result && ( article.State == articleStateFilter );
            }
            // Todo: 分类筛选 和 标签筛选
            return result;
        }
    }
}
