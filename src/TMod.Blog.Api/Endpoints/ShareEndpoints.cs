using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using TMod.Blog.Application.Services;
using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Api.Endpoints
{
    internal static class ShareEndpoints
    {
        private static ILogger? _logger;
        public static IEndpointRouteBuilder MapShareEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/shares")
                .RequireCors("blog")
                .RequireRateLimiting("default")
                .WithDescription("分享链接接口")
                .WithGroupName("v1")
                .WithSummary("博客文章、评论的分享链接接口")
                .WithTags("articles","comments")
                .ProducesProblem(StatusCodes.Status429TooManyRequests)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            ILoggerProvider loggerProvider = app.ServiceProvider.GetRequiredService<ILoggerProvider>();
            _logger = loggerProvider.CreateLogger("TMod.Blog.Api.Share");

            group.MapGet("/{shareCode}", GetSharableContentAsync)
                .WithName("GetSharableContent")
                .WithSummary("获取分享链接内容")
                .WithDescription("这个接口可以获取分享短码的内容")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            return app;
        }

        private static async Task<Results<JsonHttpResult<object>,NotFound,StatusCodeHttpResult>> GetSharableContentAsync([FromRoute] string shareCode,IShareLinkService shareLinkService,CancellationToken token)
        {
            try
            {
                ISharable? sharable = await shareLinkService.LoadByShareAsync(shareCode, token);
                if(sharable is null )
                {
                    return TypedResults.NotFound();
                }
                ShareTargetTypeEnum shareTargetType = (sharable is Article) ? ShareTargetTypeEnum.Article:ShareTargetTypeEnum.Comment;
                return TypedResults.Json<object>(new
                {
                    targetId = ( ( BaseEntity<Guid> )sharable ).Id,
                    targetType = shareTargetType
                });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "获取分享链内容发生错误");
                throw;
            }
        }
    }
}
