using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using TMod.Blog.Data.Services;

namespace TMod.Blog.Api.Endpoints
{
    internal static class ArticleTagsEndpoints
    {
        public static WebApplication MapArticleTagsEndpoints(this WebApplication app)
        {
            ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            var group = app.MapGroup("api/v1/admin")
                .WithName("ArticleTags")
                .WithDisplayName("文章标签终结点")
                .WithDescription("文章标签终结点，包含一系列对于文章标签操作的接口")
                .WithSummary("文章标签终结点")
                .WithTags("ArticleTags");
            BuildAddTagsToArticleApi(group, loggerFactory);
            BuildRemoveTagsFromArticleApi(group, loggerFactory);
            if ( app.Environment.IsDevelopment() )
            {
                group.DisableAntiforgery();
            }
            return app;
        }

        private static RouteHandlerBuilder? BuildAddTagsToArticleApi(RouteGroupBuilder? group, ILoggerFactory loggerFactory) => group?.MapPost("Articles/{articleId:guid}/Tags", async Task<Results<Created,NotFound, StatusCodeHttpResult>> ([FromRoute]Guid articleId, [FromBody]List<string> tags, [FromServices]IArticleTagsStoreService articleTagsStoreService) =>
        {
            ILogger logger = loggerFactory.CreateLogger("AddTagsToArticle");
            try
            {
                Guid id = await articleTagsStoreService.AddTagsToArticleAsync(articleId,tags??[]);
                if(id == Guid.Empty )
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.Created();
            }
            catch ( Exception )
            {
                return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
            }
        })
            .WithName("AddTagsToArticle")
            .WithDisplayName("添加标签到文章接口")
            .WithSummary("添加标签到文章接口")
            .WithDescription("添加标签到文章接口，根据 tags 和文章进行关联");

        private static RouteHandlerBuilder? BuildRemoveTagsFromArticleApi(RouteGroupBuilder? group, ILoggerFactory loggerFactory) => group?.MapDelete("Articles/{articleId:guid}/Tags", async Task<Results<NoContent, StatusCodeHttpResult>> ([FromRoute]Guid articleId, [FromBody]List<string> tags, [FromServices]IArticleTagsStoreService articleTagsStoreService) =>
        {
            ILogger logger = loggerFactory.CreateLogger("RemoveTagsFromArticle");
            try
            {
                await articleTagsStoreService.RemoveTagsFromArticleAsync(articleId, tags ?? []);
                return TypedResults.NoContent();
            }
            catch ( Exception )
            {
                return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
            }
        })
            .WithName("RemoveTagsFromArticle")
            .WithDisplayName("从文章中移除标签接口")
            .WithSummary("从文章中移除标签接口")
            .WithDescription("从文章中移除标签接口，根据 tags 从文章中取消和标签的关联");
    }
}
