using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using TMod.Blog.Application.Common.Results;
using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Article;
using TMod.Blog.Application.Services;
using TMod.Blog.Domain.Entities;

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
            ILoggerProvider loggerProvider = app.ServiceProvider.GetRequiredService<ILoggerProvider>();
            _logger = loggerProvider.CreateLogger("TMod.Blog.Api.Article");
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
            group.MapGet("/{slug}", GetArticleBySlugAsync)
                .WithName("GetArticleBySlug")
                .WithSummary("根据 Slug 获取文章")
                .WithDescription("这个接口会根据 Slug 去查找文章并关联数据")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            group.MapGet("/", PagingArticleAsync)
                .WithName("PagingArticle")
                .WithSummary("分页查询文章")
                .WithDescription("这个接口可以分页查询和筛选文章列表")
                .Produces(StatusCodes.Status200OK);
            group.MapPatch("/{articleId:guid}/category", PatchArticleCategoryAsync)
                .WithName("ChangeArticleCategory")
                .WithSummary("修改文章分类")
                .WithDescription("这个接口会修改对应文章的分类")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest);
            group.MapPatch("/{articleId:guid}/is-comment-enabled",PatchArticleIsCommentEnabledAsync)
                .WithName("SetArticleCommentEnable")
                .WithSummary("修改文章是否允许评论")
                .WithDescription("这个接口可以修改文章是否允许评论")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            group.MapPatch("/{articleId:guid}/is-share-enabled",PatchArticleIsShareEnabledAsync)
                .WithName("SetArticleShareEnable")
                .WithSummary("修改文章是否允许分享")
                .WithDescription("这个接口可以修改文章是否允许分享")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            group.MapPatch("/{articleId:guid}/tags", PatchArticleTagsAsync)
                .WithName("SetArticleTags")
                .WithSummary("修改文章标签")
                .WithDescription("这个接口可以修改文章标签")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            group.MapPut("/{articleId:guid}", UpdateArticleAsync)
                .WithName("UpdateArticle")
                .WithSummary("更新文章")
                .WithDescription("这个接口可以全量幂等的更新文章")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest);
            group.MapHealthChecks("articles-api-health");
            return app;
        }

        private static async Task<Results<CreatedAtRoute<ArticleDto>, ValidationProblem, StatusCodeHttpResult>> CreateArticleAsync([FromBody] CreateArticleRequest request, IValidator<CreateArticleRequest> validator, IArticleService articleService, CancellationToken token)
        {
            var validationResult = await validator.ValidateAsync(request,token);
            if ( !validationResult.IsValid )
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var article = await articleService.CreateArticleAsync(request,token);
                if ( article is null )
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

        private static async Task<Results<JsonHttpResult<ArticleDto>, NotFound<string>, StatusCodeHttpResult>> GetArticleByIdAsync([FromRoute] Guid articleId, IArticleService articleService, CancellationToken token)
        {
            try
            {
                var article = await articleService.GetArticleDetailAsync(articleId,token);
                if ( article is null )
                {
                    return TypedResults.NotFound("文章不存在");
                }
                return TypedResults.Json(article);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "加载文章发生错误，文章Id:{}", articleId);
                //return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        private static async Task<Results<JsonHttpResult<ArticleDto>, NotFound<string>, StatusCodeHttpResult>> GetArticleBySlugAsync([FromRoute] string slug, IArticleService articleService, CancellationToken token)
        {
            try
            {
                var article = await articleService.GetArticleDetailAsync(slug,token);
                if ( article is null )
                {
                    return TypedResults.NotFound("文章不存在");
                }
                return TypedResults.Json(article);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "加载文章发生错误，文章Slug:{}", slug);
                throw;
            }
        }

        private static async Task<Results<JsonHttpResult<Result>, StatusCodeHttpResult>> PagingArticleAsync([FromServices] IArticleService articleService, [FromQuery]Guid? categoryId = null, [FromQuery]Guid? tagId = null, [FromQuery] string? keyword = null, [FromQuery] ArticleStatusEnum status = (ArticleStatusEnum.Unpublished | ArticleStatusEnum.Draft | ArticleStatusEnum.Published), [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, CancellationToken token = default)
        {
            try
            {
                var articleList = await articleService.PagingArticleAsync(categoryId,tagId,keyword,status,pageIndex,pageSize,token);
                return TypedResults.Json((Result)articleList);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "分页查询文章发生错误");
                return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task<Results<CreatedAtRoute<ArticleDto>,ValidationProblem,BadRequest<string>,StatusCodeHttpResult>> PatchArticleCategoryAsync([FromRoute]Guid articleId,[FromBody] PatchArticleCategoryRequest request,IValidator<PatchArticleCategoryRequest> validator,IArticleService articleService,CancellationToken token)
        {
            var validationResult = await validator.ValidateAsync(request,token);
            if ( !validationResult.IsValid )
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var article = await articleService.PatchArticleCategoryAsync(articleId,request,token);
                if(article is null )
                {
                    return TypedResults.BadRequest("修改文章分类失败");
                }
                return TypedResults.CreatedAtRoute<ArticleDto>(article, "GetArticleById", new { articleId = article.Id });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "更改文章分类时发生错误");
                throw;
            }
        }

        private static async Task<Results<CreatedAtRoute<ArticleDto>, BadRequest<string>, StatusCodeHttpResult>> PatchArticleIsCommentEnabledAsync([FromRoute]Guid articleId, [FromBody]PatchArticleIsCommentEnabledRequest request,IArticleService articleService,CancellationToken token)
        {
            try
            {
                var article = await articleService.PatchArticleIsCommentEnabledAsync(articleId,request,token);
                if ( article is null )
                {
                    return TypedResults.BadRequest("修改文章是否允许评论失败");
                }
                return TypedResults.CreatedAtRoute<ArticleDto>(article, "GetArticleById", new { articleId = article.Id });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "更改文章是否允许评论时发生错误");
                throw;
            }
        }

        private static async Task<Results<CreatedAtRoute<ArticleDto>, BadRequest<string>, StatusCodeHttpResult>> PatchArticleIsShareEnabledAsync([FromRoute] Guid articleId, [FromBody] PatchArticleIsShareEnabledRequest request, IArticleService articleService, CancellationToken token)
        {
            try
            {
                var article = await articleService.PatchArticleIsShareEnabledAsync(articleId,request,token);
                if ( article is null )
                {
                    return TypedResults.BadRequest("修改文章是否允许分享失败");
                }
                return TypedResults.CreatedAtRoute<ArticleDto>(article, "GetArticleById", new { articleId = article.Id });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "更改文章是否允许分享时发生错误");
                throw;
            }
        }

        private static async Task<Results<CreatedAtRoute<ArticleDto>, BadRequest<string>, StatusCodeHttpResult>> PatchArticleTagsAsync([FromRoute]Guid articleId, [FromBody]PatchArticleTagsRequest request,IArticleService articleService,CancellationToken token)
        {
            try
            {
                var article = await articleService.PatchArticleTagsAsync(articleId,request,token);
                if(article is null )
                {
                    return TypedResults.BadRequest("修改文章标签失败");
                }
                return TypedResults.CreatedAtRoute<ArticleDto>(article, "GetArticleById", new { articleId = article.Id });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "修改文章标签时发生错误");
                throw;
            }
        }

        private static async Task<Results<CreatedAtRoute<ArticleDto>, ValidationProblem, BadRequest<string>, StatusCodeHttpResult>> UpdateArticleAsync([FromRoute]Guid articleId, [FromBody]UpdateArticleRequest request,IValidator<UpdateArticleRequest> validator,IArticleService articleService,CancellationToken token)
        {
            var validationResults = await validator.ValidateAsync(request,token);
            if ( !validationResults.IsValid )
            {
                return TypedResults.ValidationProblem(validationResults.ToDictionary());
            }

            try
            {
                var article = await articleService.UpdateArticleAsync(articleId,request,token);
                if(article is null )
                {
                    return TypedResults.BadRequest("更新文章失败");
                }
                return TypedResults.CreatedAtRoute(article, "GetArticleById", new { articleId = article.Id });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "更新文章发生错误");
                throw;
            }
        }
    }
}
