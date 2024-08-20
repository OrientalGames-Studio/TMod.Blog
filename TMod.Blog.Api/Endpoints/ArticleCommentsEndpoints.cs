using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using TMod.Blog.Api.Controllers.Admin;
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
                .WithName("ArticleComments")
                .WithSummary("文章评论管理员终结点")
                .WithTags("ArticleComments")
                .WithDisplayName("文章评论管理员终结点")
                .WithDescription("文章评论管理员终结点，包含一系列对于文章评论的管理员操作");
            BuildUpdateArticleCommentIsEnabledApi(group, loggerFactory);
            BuildBatchUpdateArticleCommentIsEnabledApi(group, loggerFactory);
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
    }
}
