using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Data.Services;

namespace TMod.Blog.Api.Endpoints
{
    internal static class ArticleContentEndpoints
    {
        internal static WebApplication MapArticleContentEndpoints(this WebApplication app)
        {
            ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            var group = app.MapGroup("api/v1")
                .WithName("ArticleContent")
                .WithDisplayName("文章内容终结点")
                .WithSummary("文章内容终结点")
                .WithTags("ArticleContent")
                .WithDescription("文章内容终结点，包含一系列对于文章内容操作的接口");
            BuildLoadArticleContentApi(group, loggerFactory);
            return app;
        }

        private static RouteHandlerBuilder? BuildLoadArticleContentApi(RouteGroupBuilder? group, ILoggerFactory loggerFactory) => group?.MapGet("Articles/{articleId:guid}/Content", async Task<Results<Ok<ArticleContentViewModel>,NotFound, StatusCodeHttpResult>> ([FromRoute]Guid articleId, [FromServices]IArticleStoreService articleStoreService) =>
        {
            ILogger logger = loggerFactory.CreateLogger("LoadArticleContent");
            try
            {
                ArticleViewModel? article = await articleStoreService.GetArticleByIdAsync(articleId);
                if(article is null )
                {
                    return TypedResults.NotFound();
                }
                ArticleContentViewModel articleContent = articleStoreService.GetArticleContentByArticleId(article.Id);
                return TypedResults.Ok(articleContent);
            }
            catch ( Exception ex )
            {
                logger.LogError(ex, $"加载文章的内容时发生异常");
                return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
            }
        })
            .WithName("LoadArticleContent")
            .WithDisplayName("加载文章正文内容接口")
            .WithSummary("加载文章正文内容接口")
            .WithDescription("加载文章正文内容接口，根据文章 id 获取文章的正文数据");
    }
}
