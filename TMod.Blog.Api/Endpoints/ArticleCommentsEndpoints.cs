using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using TMod.Blog.Api.Controllers.Admin;
using TMod.Blog.Api.Tools.ModelBinders;
using TMod.Blog.Data.Models.DTO.Articles;
using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Data.Services;

namespace TMod.Blog.Api.Endpoints
{
    internal static class ArticleCommentsEndpoints
    {
        public static WebApplication MapArticleCommentsEndpoints(this WebApplication app)
        {
            ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            var group = app.MapGroup("api/v1/admin")
                .WithName("ArticleComments_Admin")
                .WithSummary("文章评论管理员终结点")
                .WithTags("ArticleComments")
                .WithDisplayName("文章评论管理员终结点")
                .WithDescription("文章评论管理员终结点，包含一系列对于文章评论的管理员操作");
            BuildUpdateArticleCommentIsEnabledApi(group, loggerFactory);
            BuildBatchUpdateArticleCommentIsEnabledApi(group, loggerFactory);
            var normalGroup = app.MapGroup("api/v1")
                .WithName("ArticleComments")
                .WithSummary("文章评论终结点")
                .WithTags("ArticleComments")
                .WithDisplayName("文章评论终结点")
                .WithDescription("文章评论终结点，包含一系列对于文章评论的操作");
            BuildLoadArticleCommentsApi(normalGroup, loggerFactory);
            BuildReplyCommentApi(normalGroup, loggerFactory);
            return app;
        }

        private static RouteHandlerBuilder? BuildUpdateArticleCommentIsEnabledApi(RouteGroupBuilder? group, ILoggerFactory loggerFactory) => group?.MapPatch("Articles/{articleId:guid}/comments/state", async Task<Results<CreatedAtRoute<ArticleViewModel>, NotFound, StatusCodeHttpResult>> ([FromRoute] Guid articleId, [FromBody] bool isEnabled, [FromServices] IArticleStoreService articleStoreService) =>
        {
            ILogger logger = loggerFactory.CreateLogger("UpdateArticleCommentIsEnabled");
            try
            {
                Guid id = await articleStoreService.UpdateArticleCommentEnabledFlagAsync(articleId, isEnabled);
                if ( id == Guid.Empty )
                {
                    return TypedResults.NotFound();
                }
                ArticleViewModel? article = await articleStoreService.GetArticleByIdAsync(id);
                return TypedResults.CreatedAtRoute(article, nameof(ArticlesController.GetArticleByIdAsync), new { id = article?.Id ?? id });
            }
            catch ( Exception ex )
            {
                logger?.LogError(ex, $"修改文章{articleId}的控评状态至{isEnabled}发生异常");
                return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
            }
        })
            .WithDisplayName("修改文章控评状态接口")
            .WithDescription("用于修改文章是否允许评论的状态")
            .WithSummary("修改文章控评状态接口")
            .WithName("UpdateArticleCommentIsEnabled");

        private static RouteHandlerBuilder? BuildBatchUpdateArticleCommentIsEnabledApi(RouteGroupBuilder? group, ILoggerFactory loggerFactory) => group?.MapPatch("Articles/Comments/State", async Task<Results<Ok, StatusCodeHttpResult>> ([FromBody] Dictionary<Guid, bool> stateDic, [FromServices] IArticleStoreService articleStoreService) =>
        {
            ILogger logger = loggerFactory.CreateLogger("BatchUpdateArticleCommentIsEnabled");
            try
            {
                bool result = await articleStoreService.BatchUpdateArticleCommentEnabledFlagAsync(stateDic);
                if ( !result )
                {
                    return TypedResults.StatusCode(StatusCodes.Status422UnprocessableEntity);
                }
                return TypedResults.Ok();
            }
            catch ( Exception ex )
            {
                logger?.LogError(ex, $"批量修改文章的控评状态时发生异常");
                return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
            }
        })
            .WithDisplayName("批量修改文章控评状态接口")
            .WithDescription("用于批量修改文章是否允许评论的状态")
            .WithSummary("批量修改文章控评状态接口")
            .WithName("BatchUpdateArticleCommentIsEnabled");

        private static RouteHandlerBuilder? BuildLoadArticleCommentsApi(RouteGroupBuilder? group, ILoggerFactory loggerFactory) => group?.MapGet("Articles/{articleId:guid}/Comments/{commentId:guid?}", Results<ContentHttpResult, StatusCodeHttpResult, NotFound> ([FromRoute] Guid articleId, [FromServices] IArticleStoreService articleStoreServices, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, [FromRoute] Guid? commentId = null, [FromQuery] bool showAll = false) =>
        {
            ILogger logger = loggerFactory.CreateLogger("LoadArticleComments");
            try
            {
                IEnumerable<ArticleCommentViewModel?> comments = articleStoreServices.PaingLoadArticleComments(articleId, pageIndex, pageSize,out int dataCount,out int pageCount,commentId,showAll );
                object pagingResult = new
                {
                    pageIndex,
                    pageSize,
                    dataCount,
                    pageCount,
                    data = comments
                };
                return TypedResults.Text(JsonSerializer.Serialize(pagingResult), MediaTypeHeaderValue.Parse("text/plain").MediaType, Encoding.UTF8, StatusCodes.Status200OK);
                //return TypedResults.Ok<object>(JsonSerializer.Serialize(pagingResult));
            }
            catch ( Exception ex )
            {
                logger.LogError(ex, $"根据文章编号加载文章评论列表时发生异常");
                return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
            }
        })
            .WithDisplayName("根据文章编号加载文章评论接口")
            .WithDescription("用于加载文章下的评论")
            .WithSummary("根据文章编号加载文章评论接口")
            .WithName("LoadArticleComments");

        private static RouteHandlerBuilder? BuildReplyCommentApi(RouteGroupBuilder? group, ILoggerFactory loggerFactory) => group?.MapPost("Articles/{articleId:guid}/Comments/{commentId:guid?}", async Task<Results<Created, StatusCodeHttpResult, NotFound,BadRequest<string>>> ([FromServices]IArticleStoreService articleStoreService, ReplyCommentModel? model, [FromRoute]Guid? articleId, [FromRoute]Guid? commentId) =>
        {
            ILogger logger = loggerFactory.CreateLogger("CommitReply");
            try
            {
                if ( model is null )
                {
                    return TypedResults.BadRequest("评论数据不允许为空");
                }
                List<ValidationResult> errors = new List<ValidationResult>();
                ValidationContext context = new ValidationContext(model);
                if ( !Validator.TryValidateObject(model, context, errors) )
                {
                    return TypedResults.BadRequest(string.Join(Environment.NewLine, errors.Select(p => p.ErrorMessage)));
                }
                ArticleCommentViewModel? comment = await articleStoreService.ReplyCommentAsync(model);
                if ( comment is null )
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.Created();
            }
            catch ( Exception ex )
            {
                logger.LogError(ex, $"发表评论时发生异常");
                return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
            }
        })
            .WithDisplayName("提交文章评论接口")
            .WithDescription("用于提交文章评论")
            .WithSummary("提交文章评论接口")
            .WithName("ReplyComment");
    }
}
