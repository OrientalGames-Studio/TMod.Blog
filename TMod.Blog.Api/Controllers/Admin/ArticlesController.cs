using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

using TMod.Blog.Api.Tools.ModelBinders;
using TMod.Blog.Data.Models.DTO.Articles;
using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Data.Models.ViewModels.Categories;
using TMod.Blog.Data.Services;

namespace TMod.Blog.Api.Controllers.Admin
{
    [Route("api/v1/admin/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ILogger<ArticlesController> _logger;
        private readonly IArticleStoreService _articleStoreService;
        private readonly Constants _constants;

        public ArticlesController(ILogger<ArticlesController> logger, IArticleStoreService articleStoreService, Constants constants)
        {
            _logger = logger;
            _articleStoreService = articleStoreService;
            _constants = constants;
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
            , [FromQuery] DateOnly? articlePublishedDateEndFilter = null  // 最大发布日期筛选
            , [FromQuery] DateOnly? articleLastEditDateEndFilter = null  // 最大最后发布日期筛选
            , [FromQuery] DateOnly? articleCreateDateFilter = null  // 创建日期筛选
            , [FromQuery] DateOnly? articleCreateDateEndFilter = null // 最大创建日期筛选
            , [FromQuery] bool? articleCommentStateFilter = null
            )
        {
            object pagingResult;
            try
            {
                IQueryable<ArticleViewModel?> articles = _articleStoreService.Paging(pageIndex,pageSize,out int dataCount,out int pageCount,article=>FilterArticleV2(article,articleTitleFilter,articleSnapshotFilter,articleCategoryFilter,articleTagsFilter,articleCreateDateFilter,articleCreateDateEndFilter,articleLastEditDateFilter,articleLastEditDateEndFilter,articlePublishedDateFilter,articlePublishedDateEndFilter,articleStateFilter,articleCommentStateFilter));
                articles.OrderBy(p => p!.State)
                    .ThenByDescending(p => p!.LastEditDate ?? p!.UpdateDate ?? p!.CreateDate);
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

        private bool FilterArticleV2(ArticleViewModel? article, string? articleTitleFilter, string? articleSnapshotFilter, string? articleCategoryFilter, string? articleTagsFilter, DateOnly? articleCreateDateFilter, DateOnly? articleCreateDateEndFilter, DateOnly? articleLastEditDateFilter, DateOnly? articleLastEditDateEndFilter, DateOnly? articlePublishedDateFilter, DateOnly? articlePublishedDateEndFilter, ArticleStateEnum? articleStateFilter, bool? articleCommentStateFilter)
        {
            bool result = true;
            string[] categories = articleCategoryFilter?.Split(_constants.SplitStringChars,StringSplitOptions.RemoveEmptyEntries)??[];
            string[] tags = articleTagsFilter?.Split(_constants.SplitStringChars, StringSplitOptions.RemoveEmptyEntries)??[];
            if ( article is null )
            {
                return false;
            }
            #region 创建时间筛选
            if ( articleCreateDateFilter is not null && articleCreateDateEndFilter is not null )
            {
                result = result &&
                    DateOnly.FromDateTime(article.CreateDate) >= articleCreateDateFilter &&
                    DateOnly.FromDateTime(article.CreateDate) < articleCreateDateEndFilter;
            }
            else if ( articleCreateDateFilter is not null && articleCreateDateEndFilter is null )
            {
                result = result &&
                    DateOnly.FromDateTime(article.CreateDate) >= articleCreateDateFilter;
            }
            else if ( articleCreateDateFilter is null && articleCreateDateEndFilter is not null )
            {
                result = result &&
                    DateOnly.FromDateTime(article.CreateDate) < articleCreateDateEndFilter;
            }
            #endregion
            #region 编辑时间筛选
            if ( articleLastEditDateFilter is not null && articleLastEditDateEndFilter is not null )
            {
                result = result &&
                    DateOnly.FromDateTime(article.LastEditDate.GetValueOrDefault()) >= articleLastEditDateFilter &&
                    DateOnly.FromDateTime(article.LastEditDate.GetValueOrDefault()) < articleLastEditDateEndFilter;
            }
            else if ( articleLastEditDateFilter is not null && articleLastEditDateEndFilter is null )
            {
                result = result &&
                    DateOnly.FromDateTime(article.LastEditDate.GetValueOrDefault()) >= articleLastEditDateFilter;
            }
            else if ( articleLastEditDateFilter is null && articleLastEditDateEndFilter is not null )
            {
                result = result &&
                    DateOnly.FromDateTime(article.LastEditDate.GetValueOrDefault()) < articleLastEditDateEndFilter;
            }
            #endregion
            #region 发布时间筛选
            if ( articlePublishedDateFilter is not null && articlePublishedDateEndFilter is not null )
            {
                result = result &&
                    DateOnly.FromDateTime(article.PublishDate.GetValueOrDefault()) >= articlePublishedDateFilter &&
                    DateOnly.FromDateTime(article.PublishDate.GetValueOrDefault()) < articlePublishedDateEndFilter;
            }
            else if ( articlePublishedDateFilter is not null && articlePublishedDateEndFilter is null )
            {
                result = result &&
                    DateOnly.FromDateTime(article.PublishDate.GetValueOrDefault()) >= articlePublishedDateFilter;
            }
            else if ( articlePublishedDateFilter is null && articlePublishedDateEndFilter is not null )
            {
                result = result &&
                    DateOnly.FromDateTime(article.PublishDate.GetValueOrDefault()) < articlePublishedDateEndFilter;
            }
            #endregion
            #region 文章状态筛选
            if ( articleStateFilter is not null )
            {
                //result = result && ( ((short)articleStateFilter  & ((short)article.State+1) ) == ((short)article.State) );
                result = result && (( article.State & articleStateFilter ) != 0);
            }
            #endregion
            #region 标签筛选
            if ( tags.Any() )
            {
                result = result && ( article.Tags.Any(t => Array.IndexOf(tags, t.Tag) > -1) );
            }
            #endregion
            #region 分类筛选
            if ( categories.Any() )
            {
                result = result && ( article.Categories.Any(c => Array.IndexOf(categories, c.Category) > -1) );
            }
            #endregion
            if ( !string.IsNullOrWhiteSpace(articleSnapshotFilter) )
            {
                result = result && ( article.Snapshot?.Contains(articleSnapshotFilter) == true );
            }
            if ( !string.IsNullOrWhiteSpace(articleTitleFilter) )
            {
                result = result && article.Title.Contains(articleTitleFilter);
            }
            if ( articleCommentStateFilter.HasValue )
            {
                result = result && article.IsCommentEnabled == articleCommentStateFilter;
            }
            return result;
        }

        [ActionName(nameof(GetArticleByIdAsync))]
        [HttpGet("{id:guid}", Name = nameof(GetArticleByIdAsync))]
        public async Task<IActionResult> GetArticleByIdAsync([FromRoute] Guid id)
        {
            ArticleViewModel? viewModel = await _articleStoreService.GetArticleByIdAsync(id);
            if ( viewModel is null )
            {
                return NotFound();
            }
            return Ok(viewModel);
        }

        private bool FilterArticle(ArticleViewModel? article, string? articleTitleFilter, string? articleSnapshotFilter, string? articleCategoryFilter, string? articleTagsFilter, ArticleStateEnum? articleStateFilter, DateOnly? articlePublishedDateFilter, DateOnly? articleLastEditDateFilter)
        {
            bool result = true;
            string[] categories = [];
            string[] tags = [];
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
            if ( !string.IsNullOrWhiteSpace(articleCategoryFilter) )
            {
                categories = articleCategoryFilter.Split(_constants.SplitStringChars, StringSplitOptions.RemoveEmptyEntries);
                result = result && ( article.Categories is not null && article.Categories.Count > 0 ) && article.Categories.Any(p => Array.IndexOf(categories, p.Category) > -1);
            }
            if ( !string.IsNullOrWhiteSpace(articleTagsFilter) )
            {
                tags = articleTagsFilter.Split(_constants.SplitStringChars, StringSplitOptions.RemoveEmptyEntries);
                result = result && ( article.Tags is not null && article.Tags.Count > 0 ) && article.Tags.Any(p => Array.IndexOf(tags, p.Tag) > -1);
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
            return result;
        }



        [HttpPost]
        public async Task<IActionResult> CreateArticleAsync([FromForm] AddArticleModel model, [FromForm] IFormFileCollection archives)
        {
            if ( !ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }
            List<AddArticleArchiveModel> archiveModels = new List<AddArticleArchiveModel>();
            if ( archives is not null )
            {
                foreach ( IFormFile uploadedFile in archives )
                {
                    MemoryStream ms = new MemoryStream();
                    await uploadedFile.CopyToAsync(ms);
                    AddArticleArchiveModel archiveModel = new AddArticleArchiveModel(ms);
                    archiveModel.MIMEType = uploadedFile.ContentType;
                    archiveModel.FileName = uploadedFile.FileName;
                    archiveModel.FileSizeMB = ( double )ms.Length / 1024 / 1024;
                    archiveModels.Add(archiveModel);
                }
            }
            Guid? articleId = await _articleStoreService.CreateArticleAsync(model, archiveModels);
            if ( articleId is null || articleId.Value == Guid.Empty )
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "由于未知原因创建文章失败");
            }
            return CreatedAtAction(nameof(GetArticleByIdAsync), new { id = articleId }, articleId);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateArticleAsync([FromRoute] Guid id, [ModelBinder(typeof(UpdateArticleModelBinder))] UpdateArticleModel model, [FromForm] IFormFileCollection archives)
        {
            if ( !ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }
            ArticleViewModel? metaData = await _articleStoreService.GetArticleByIdAsync(id);
            if ( metaData is null )
            {
                return BadRequest("修改失败，元数据不存在");
            }
            if ( metaData.Id != model.Meta!.Article!.Id || metaData.Id != model.Meta!.ArticleContent!.ArticleId )
            {
                return BadRequest($"修改失败，元数据编号和修改数据编号不一致");
            }
            if ( metaData.Version != model.Meta!.Article!.Version )
            {
                return BadRequest("修改失败，元数据已被修改，请重新加载数据后再试");
            }
            IEnumerable<CategoryViewModel> deletedCategories = model.Meta.Article.Categories?.Where(p=>p.IsRemove)??[];
            IEnumerable<CategoryViewModel> addedCategories = model.Categories.Where(p=>!p.IsRemove);
            IEnumerable<ArticleTagViewModel> deletedTags = model.Meta.Article.Tags?.Where(p=>p.IsRemove)??[];
            IEnumerable<ArticleTagViewModel> addedTags = model.Tags.Where(p=>!p.IsRemove);
            IEnumerable<ArticleArchiveViewModel> deletedArchives = model.Meta.Archives?.Where(p=>p.IsRemove)??[];
            List<AddArticleArchiveModel> addedArchives = [];
            foreach ( IFormFile archive in archives )
            {
                MemoryStream ms = new MemoryStream();
                await archive.CopyToAsync(ms);
                AddArticleArchiveModel archiveModel = new AddArticleArchiveModel(ms);
                archiveModel.MIMEType = archive.ContentType;
                archiveModel.FileName = archive.FileName;
                archiveModel.FileSizeMB = ( double )ms.Length / 1024 / 1024;
                addedArchives.Add(archiveModel);
            }
            try
            {
                Guid articleId = await _articleStoreService.UpdateArticleAsync(metaData.Id,model.Meta.Article,model.Meta.ArticleContent,deletedCategories,addedCategories,deletedTags,addedTags,deletedArchives,addedArchives);
                return CreatedAtAction(nameof(GetArticleByIdAsync), new { id = articleId }, articleId);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"修改编号为{id}的文章失败，入参:{JsonSerializer.Serialize(model)}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"修改文章{id}数据失败");
            }
        }

        [HttpPatch("{id:guid}/state")]
        public async Task<IActionResult> UpdateArticleStateAsync([FromRoute] Guid id, [FromBody] UpdateArticleStateModel model)
        {
            if ( !ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }
            if(((short)model.State & ((short)model.State) - 1) != 0 )
            {
                return BadRequest($"");
            }
            try
            {
                Guid articleId = await _articleStoreService.UpdateArticleStateAsync(id,model.State);
                if ( articleId == Guid.Empty )
                {
                    return NotFound();
                }
                return CreatedAtAction(nameof(GetArticleByIdAsync), new { id = articleId }, articleId);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"修改文章{id}的状态为{model.State}时发生异常");
                return StatusCode(StatusCodes.Status500InternalServerError, $"修改文章{id}的状态失败");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveArticleByIdAsync([FromRoute] Guid id)
        {
            try
            {
                await _articleStoreService.RemoveArticleByIdAsync(id);
                return NoContent();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"删除文章{id}时发生异常");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> BatchRemoveArticleAsync([FromBody] BatchRemoveArticleModel model)
        {
            if ( !ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _articleStoreService.BatchRemoveArticleAsync(model);
                return NoContent();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"批量删除文章时发生异常");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch("state")]
        public async Task<IActionResult> BatchUpdateArticleStateAsync([FromBody] BatchUpdateArticleStateModel model)
        {
            if ( !ModelState.IsValid || model.ArticleStates.Count == 0)
            {
                return BadRequest(ModelState);
            }
            try
            {
                bool result = await _articleStoreService.BatchUpdateArticleStateAsync(model.ArticleStates);
                if ( !result )
                {
                    return UnprocessableEntity();
                }
                return Ok();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"批量修改文章的状态时发生异常");
                return StatusCode(StatusCodes.Status500InternalServerError, $"批量修改文章的状态失败");
            }
        }
    }
}
