using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using TMod.Blog.Data.Models.DTO.Categories;
using TMod.Blog.Data.Services;

namespace TMod.Blog.Api.Endpoints
{
    internal static class ArticleCategoriesEndpoints
    {
        public static WebApplication MapArticleCategoriesEndpoints(this WebApplication app)
        {
            ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            var group = app.MapGroup("api/v1/admin")
                .WithName("ArticleCategories")
                .WithDisplayName("文章分类管理员终结点")
                .WithSummary("文章分类管理员终结点")
                .WithTags("ArticleCategories")
                .WithDescription("文章分类管理员终结点，包含一系列对于文章分类的管理员操作");
            BuildAddArticleCategoriesApi(group, loggerFactory);
            BuildSubstractArticleCategoriesApi(group,loggerFactory);
            if(app.Environment.IsDevelopment())
            {
                group?.DisableAntiforgery();
            }
            return app;
        }

        private static RouteHandlerBuilder? BuildAddArticleCategoriesApi(RouteGroupBuilder? group, ILoggerFactory loggerFactory) => group?.MapPost("Articles/{articleId:guid}/Categories", async Task<Results<Created,NotFound, StatusCodeHttpResult>> ([FromRoute]Guid articleId, [FromBody]List<string> categories, [FromServices]ICategoryStoreService categoryStoreService) =>
        {
            ILogger logger = loggerFactory.CreateLogger("AddArticleCategories");
            try
            {
                Guid id = await categoryStoreService.AppendCategoriesToArticleAsync(articleId, categories ?? []);
                if (id == Guid.Empty )
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
            .WithName("AddArticleCategories")
            .WithDisplayName("添加分类到文章")
            .WithSummary("添加分类到文章")
            .WithDescription("添加分类到文章接口，根据 categories 参数自动创建分类并和文章关联");

        private static RouteHandlerBuilder? BuildSubstractArticleCategoriesApi(RouteGroupBuilder? group, ILoggerFactory loggerFactory) => group?.MapDelete("Articles/{articleId:guid}/Categories", async Task<Results<NoContent, StatusCodeHttpResult>> ([FromRoute]Guid articleId, [FromBody]List<string> categories, [FromServices]ICategoryStoreService categoryStoreService) =>
        {
            ILogger logger = loggerFactory.CreateLogger("SubstractArticleCategories");
            try
            {
                await categoryStoreService.SubstractCategoriesFromArticleAsync(articleId, categories ?? []);
                return TypedResults.NoContent();
            }
            catch ( Exception )
            {
                return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
            }
        })
            .WithName("SubstractArticleCategories")
            .WithDisplayName("从文章中移除分类")
            .WithSummary("从文章中移除分类")
            .WithDescription("从文章中移除分类接口，根据 categories 参数移除分类和文章的关联");
    }
}
