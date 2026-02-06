using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using TMod.Blog.Api.Extensions;
using TMod.Blog.Application.Common.Results;
using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Comment;
using TMod.Blog.Application.Requests.Share;
using TMod.Blog.Application.Services;
using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Api.Endpoints
{
    internal static class CommentEndpoints
    {
        private static ILogger? _logger;
        public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/comments")
                .RequireCors("blog")
                .RequireRateLimiting("default")
                .WithDescription("评论接口")
                .WithGroupName("v1")
                .WithSummary("博客的评论接口")
                .WithTags("comments")
                .ProducesProblem(StatusCodes.Status429TooManyRequests)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
            ILoggerProvider loggerProvider = app.ServiceProvider.GetRequiredService<ILoggerProvider>();
            _logger = loggerProvider.CreateLogger("TMod.Blog.Api.Comment");

            group.MapPost("/", CreateCommentAsync)
                .WithName("CreateComment")
                .WithSummary("发表评论")
                .WithDescription("这个接口可以发表一个评论给文章或者评论")
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            group.MapGet("/{commentId:guid}", GetCommentByIdAsync)
                .WithName("GetCommentById")
                .WithSummary("获取评论")
                .WithDescription("这个接口可以获取一个评论的数据")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            group.MapGet("/{articleId:guid}/comments", PagingCommentByArticleAsync)
                .WithName("GetArticleComments")
                .WithSummary("获取文章的顶级评论")
                .WithDescription("这个接口可以获取文章所有评论的顶级评论（又叫一楼）")
                .Produces(StatusCodes.Status200OK);
            group.MapGet("/{commentId:guid}/replies", PagingCommentByCommentAsync)
                .WithName("GetCommentReply")
                .WithSummary("获取评论的回复")
                .WithDescription("这个接口可以获取评论的所有回复")
                .Produces(StatusCodes.Status200OK);
            group.MapHealthChecks("comments-api-health");

            // 扩展
            group.MapPost("/{commentId:guid}/favorites", FavoriteCommentAsync)
                .WithName("FavoriteComment")
                .WithSummary("给评论点赞")
                .WithDescription("这个接口可以给文章点赞")
                .Produces(StatusCodes.Status200OK);

            group.MapGet("/{commentId:guid}/favorites", CountCommentFavoriteAsync)
                .WithName("CountCommentFavorites")
                .WithSummary("获取评论的点赞数量")
                .WithDescription("这个接口可以获取评论被几个人点赞过")
                .Produces(StatusCodes.Status200OK);

            group.MapDelete("/{commentId:guid}/favorites", DisfavoriteCommentAsync)
                .WithName("DisfavoriteComment")
                .WithSummary("取消给评论点赞")
                .WithDescription("这个接口可以取消给评论的点赞")
                .Produces(StatusCodes.Status204NoContent);

            group.MapPost("/{commentId:guid}/shares", ShareCommentAsync)
                .WithName("ShareComment")
                .WithSummary("分享评论")
                .WithDescription("这个接口会创建一个分享短码，通过分享短码可以访问到文章")
                .Produces(StatusCodes.Status200OK)
                .ProducesValidationProblem();

            return app;
        }

        private static async Task<Results<CreatedAtRoute<CommentDto>,ValidationProblem,StatusCodeHttpResult>> CreateCommentAsync([FromBody]CreateCommentRequest request,IValidator<CreateCommentRequest> validator,ICommentService commentService,IHttpContextAccessor httpContextAccessor,CancellationToken token)
        {
            string ip = httpContextAccessor.GetClientIp();
            var validationResults = await validator.ValidateAsync(request,token);
            if ( !validationResults.IsValid )
            {
                return TypedResults.ValidationProblem(validationResults.ToDictionary());
            }
            try
            {
                var comment = await commentService.CreateCommentAsync(request,ip,token);
                if(comment is null )
                {
                    _logger?.LogCritical("因不明原因导致评论创建返回空值");
                    return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
                }
                return TypedResults.CreatedAtRoute(comment, "GetCommentById", new { commentId = comment.Id });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "发表评论发生错误");
                throw;
            }
        }

        private static async Task<Results<JsonHttpResult<CommentDto>,NotFound<string>,StatusCodeHttpResult>> GetCommentByIdAsync([FromRoute]Guid commentId,ICommentService commentService,CancellationToken token)
        {
            try
            {
                var comment = await commentService.GetCommentByIdAsync(commentId,token);
                if(comment is null )
                {
                    return TypedResults.NotFound("评论不存在");
                }
                return TypedResults.Json(comment);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "获取评论发生错误");
                throw;
            }
        }

        private static async Task<Results<CreatedAtRoute<CommentDto>,ValidationProblem,BadRequest<string>,StatusCodeHttpResult>> PutCommentAsync([FromRoute]Guid commentId, [FromBody]UpdateCommentRequest request,IValidator<UpdateCommentRequest> validator,ICommentService commentService,IHttpContextAccessor httpContextAccessor,CancellationToken token)
        {
            throw new NotImplementedException("服务里没有设计修改评论的接口，所以不处理修改评论请求");
        }

        private static async Task<Results<JsonHttpResult<Result>,StatusCodeHttpResult>> PagingCommentByArticleAsync([FromRoute]Guid articleId,[FromServices]ICommentService commentService, [FromQuery]int pageIndex = 1, [FromQuery]int pageSize = 20, [FromQuery]SortRuleEnum sortRule = SortRuleEnum.Latest, CancellationToken token = default)
        {
            try
            {
                var commentList = await commentService.PagingCommentsByArticleAsync(articleId,pageIndex,pageSize,sortRule,token);
                return TypedResults.Json(( Result )commentList);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "分页查询文章{}的评论失败", articleId);
                throw;
            }
        }

        private static async Task<Results<JsonHttpResult<Result>, StatusCodeHttpResult>> PagingCommentByCommentAsync([FromRoute]Guid commentId, [FromServices]ICommentService commentService, [FromQuery]int pageIndex = 1, [FromQuery]int pageSize = 20, [FromQuery] SortRuleEnum sortRule = SortRuleEnum.Latest, CancellationToken token = default)
        {
            try
            {
                var commentList = await commentService.PagingCommentsByCommentAsync(commentId,pageIndex,pageSize,sortRule,token);
                return TypedResults.Json(( Result )commentList);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "分页查询评论{}的回复失败", commentId);
                throw;
            }
        }

        private static async Task<Results<Ok,StatusCodeHttpResult>> FavoriteCommentAsync([FromRoute]Guid commentId,IFavoriteService favoriteService,IHttpContextAccessor httpContextAccessor,CancellationToken token)
        {
            try
            {
                string clientIp = httpContextAccessor.GetClientIp();
                string? fingerprint = httpContextAccessor.GetFingerPrint();
                if(!await favoriteService.GetCommentIsFavoritedAsync(commentId, fingerprint, clientIp, token) )
                {
                    await favoriteService.FavoriteCommentAsync(commentId, fingerprint, clientIp, token);
                }
                return TypedResults.Ok();
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "评论点赞发生错误");
                throw;
            }
        }

        private static async Task<Results<Ok<int>,StatusCodeHttpResult>> CountCommentFavoriteAsync([FromRoute]Guid commentId,IFavoriteService favoriteService,IHttpContextAccessor httpContextAccessor,CancellationToken token)
        {
            try
            {
                int count = await favoriteService.CountCommentFavoritesAsync(commentId,token);
                return TypedResults.Ok(count);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "计算评论点赞数量发生错误");
                throw;
            }
        }

        private static async Task<Results<NoContent, StatusCodeHttpResult>> DisfavoriteCommentAsync([FromRoute]Guid commentId,IFavoriteService favoriteService,IHttpContextAccessor httpContextAccessor,CancellationToken token)
        {
            try
            {
                string ip = httpContextAccessor.GetClientIp();
                string? fingerprint = httpContextAccessor.GetFingerPrint();
                bool res = await favoriteService.DisfavoriteArticleAsync(commentId,fingerprint,ip,token);
                if ( !res )
                {
                    _logger?.LogWarning("取消评论点赞时返回 false，评论 Id:{}", commentId);
                }
                return TypedResults.NoContent();
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "取消评论点赞发生错误");
                throw;
            }
        }

        private static async Task<Results<Ok<string>, ValidationProblem, StatusCodeHttpResult>> ShareCommentAsync([FromRoute]Guid commentId, [FromBody] CreateShareRequest request, IValidator<CreateShareRequest> validator, IShareLinkService shareLinkService, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator, CancellationToken token)
        {
            var validationResult = await validator.ValidateAsync(request,token);
            if ( !validationResult.IsValid )
            {
                _logger?.LogError("创建分享链时，分享预设参数校验失败：{}", validationResult.ToString());
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }
            try
            {
                DateTime now = DateTime.Now;
                DateTime? expireAt = null;
                if ( request.AutoExpire )
                {
                    expireAt = request.DaysUnit switch
                    {
                        Application.Common.Enums.DaysUnitEnum.Days => now.AddDays(request.ExpireAt),
                        Application.Common.Enums.DaysUnitEnum.Weeks => now.AddDays(request.ExpireAt * 7),
                        Application.Common.Enums.DaysUnitEnum.Months => now.AddMonths(request.ExpireAt),
                        Application.Common.Enums.DaysUnitEnum.Seasons => now.AddMonths(request.ExpireAt * 3),
                        Application.Common.Enums.DaysUnitEnum.Years => now.AddYears(request.ExpireAt),
                        _ => now.AddDays(request.ExpireAt)
                    };
                }
                string clientIp = httpContextAccessor.GetClientIp();
                string? fingerprint = httpContextAccessor.GetFingerPrint();
                var shortCode = await shareLinkService.CreateShareAsync(ShareTargetTypeEnum.Comment,commentId,clientIp,expireAt,token);
                //string url = linkGenerator.GetUriByName(httpContextAccessor.HttpContext,)
                //string? url = linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext, "GetShareArticle", new { shareCode = shortCode });
                if ( string.IsNullOrWhiteSpace(shortCode) )
                {
                    _logger?.LogCritical("生成文章分享链失败，文章 Id:{}，分享短码: {}", commentId, shortCode);
                    return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
                }
                return TypedResults.Ok(shortCode);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "分享评论时发生错误");
                throw;
            }
        }
    }
}
