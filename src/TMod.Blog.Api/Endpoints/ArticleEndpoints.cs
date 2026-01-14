using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Article;
using TMod.Blog.Application.Services;

namespace TMod.Blog.Api.Endpoints
{
    internal static class ArticleEndpoints
    {
        private static ILogger? _logger;
        internal static IEndpointRouteBuilder MapArticleEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/articles")
                .RequireCors("blog")
                .RequireRateLimiting("article-rate-limit-policy")
                .WithDescription("博客的文章接口")
                .WithGroupName("v1")
                .WithSummary("博客的文章接口")
                .WithTags("articles")
                .ProducesProblem(StatusCodes.Status429TooManyRequests)
                .ProducesProblem(StatusCodes.Status500InternalServerError) ;
            //group.MapPost("/", Test)
            //    .WithName("Test")
            //    .WithSummary("测试接口")
            //    .WithDescription("这是个测试接口")
            //    .Produces<ArticleDto>(StatusCodes.Status200OK)
            //    .ProducesProblem(StatusCodes.Status400BadRequest)
            //    .ProducesValidationProblem();
            ILoggerProvider loggerProvider = app.ServiceProvider.GetRequiredService<ILoggerProvider>();
            _logger = loggerProvider.CreateLogger("TMod.Blog.Api.Article");
            // TODO: 查文章列表，加载文章，发表文章，更新文章
            group.MapPost("/", CreateArticleAsync)
                .WithName("CreateArticle")
                .WithSummary("创建文章")
                .WithDescription("这个接口会创建一条 Article 数据")
                .Produces(StatusCodes.Status201Created)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest);
            group.MapGet("/{articleId:guid}", GetArticleByIdAsync)
                .WithName("GetArticleById")
                .WithSummary("根据 Id 获取文章")
                .WithDescription("这个接口会根据 Id 去查找文章并关联数据")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            group.MapHealthChecks("articles-api-health");
            return app;
        }

        //private static async Task<Results<Ok,BadRequest>> Test([FromBody]CreateArticleRequest createArticleRequest, [FromServices]IValidator<CreateArticleRequest> validator)
        //{
        //    return TypedResults.Ok();
        //}

        private static async Task<Results<CreatedAtRoute<ArticleDto>,ValidationProblem,StatusCodeHttpResult>> CreateArticleAsync([FromBody]CreateArticleRequest request,IValidator<CreateArticleRequest> validator,IArticleService articleService,CancellationToken token)
        {
            var validationResult = await validator.ValidateAsync(request,token);
            if ( !validationResult.IsValid )
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var article = await articleService.CreateArticleAsync(request,token);
                if(article is null )
                {
                    _logger?.LogCritical("因为不明原因导致文章创建返回空值");
                    return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
                }
                return TypedResults.CreatedAtRoute(article, "GetArticleById", new { articleId = article.Id });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "创建文章发生错误");
                throw;
                //return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task<Results<JsonHttpResult<ArticleDto>,NotFound<string>,StatusCodeHttpResult>> GetArticleByIdAsync([FromRoute]Guid articleId,IArticleService articleService,CancellationToken token)
        {
            try
            {
                var article = await articleService.GetArticleDetailAsync(articleId,token);
                if(article is null )
                {
                    return TypedResults.NotFound("文章不存在");
                }
                return TypedResults.Json(article);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "加载文章发生错误");
                //return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
    }
}
