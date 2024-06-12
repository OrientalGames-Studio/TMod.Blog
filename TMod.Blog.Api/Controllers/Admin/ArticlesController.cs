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

        public ArticlesController(ILogger<ArticlesController> logger, IArticleStoreService articleStoreService,Constants constants)
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
            )
        {
            object pagingResult;
            try
            {
                IQueryable<ArticleViewModel?> articles = _articleStoreService.Paging(pageIndex,pageSize,out int dataCount,out int pageCount,article=>FilterArticle(article,articleTitleFilter,articleSnapshotFilter,articleCategoryFilter,articleTagsFilter,articleStateFilter,articlePublishedDateFilter,articleLastEditDateFilter));
                articles.OrderBy(p=>p!.State)
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
            if(!string.IsNullOrWhiteSpace(articleCategoryFilter) )
            {
                categories = articleCategoryFilter.Split(_constants.SplitStringChars,StringSplitOptions.RemoveEmptyEntries);
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
        public async Task<IActionResult> CreateArticleAsync([FromForm]AddArticleModel model, [FromForm]IFormFileCollection archives)
        {
            if ( ! ModelState.IsValid )
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
                    archiveModel.FileSizeMB = (double)ms.Length / 1024 / 1024;
                    archiveModels.Add(archiveModel);
                }
            }
            Guid? articleId = await _articleStoreService.CreateArticleAsync(model, archiveModels);
            if(articleId is null || articleId.Value == Guid.Empty )
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "由于未知原因创建文章失败");
            }
            return CreatedAtAction(nameof(GetArticleByIdAsync), new { id = articleId },articleId);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateArticleAsync([FromRoute]Guid id,[ModelBinder(typeof(UpdateArticleModelBinder))] UpdateArticleModel model, [FromForm]IFormFileCollection archives)
        {
            if ( !ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }
            ArticleViewModel? metaData = await _articleStoreService.GetArticleByIdAsync(id);
            if(metaData is null)
            {
                return BadRequest("修改失败，元数据不存在");
            }
            if(metaData.Id != model.Meta!.Article!.Id || metaData.Id != model.Meta!.ArticleContent!.ArticleId)
            {
                return BadRequest($"修改失败，元数据编号和修改数据编号不一致");
            }
            if(metaData.Version != model.Meta!.Article!.Version )
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
        public async Task<IActionResult> UpdateArticleStateAsync([FromRoute]Guid id, [FromBody]UpdateArticleStateModel model)
        {
            if (! ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }
            try
            {
                Guid articleId = await _articleStoreService.UpdateArticleStateAsync(id,model.State);
                if(articleId ==  Guid.Empty)
                {
                    return NotFound();
                }
                return CreatedAtAction(nameof(GetArticleByIdAsync),new { id = articleId }, articleId);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"修改文章{id}的状态为{model.State}时发生异常");
                return StatusCode(StatusCodes.Status500InternalServerError, $"修改文章{id}的状态失败");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveArticleByIdAsync([FromRoute]Guid id)
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
    }
}
