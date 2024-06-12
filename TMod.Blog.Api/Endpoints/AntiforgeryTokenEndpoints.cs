using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace TMod.Blog.Api.Endpoints
{
    internal static class AntiforgeryTokenEndpoints
    {
        public static WebApplication MapAntiforgeryTokenEndpoints(this WebApplication app)
        {
            ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            var group = app.MapGroup("api/v1/Security")
                .WithName("AntiforgeryToken")
                .WithDisplayName("防伪令牌终结点")
                .WithSummary("AntiforgeryToken")
                .WithDescription("防伪令牌终结点，包含一系列防伪令牌操作")
                .WithTags("Security");
            BuildGetAntiforgeryTokenApi(group, loggerFactory);
            return app;
        }

        private static RouteHandlerBuilder? BuildGetAntiforgeryTokenApi(RouteGroupBuilder? group,ILoggerFactory loggerFactory) => group.MapGet("AntiforgeryToken", async Task<Results<Ok<AntiforgeryTokenSet>, StatusCodeHttpResult>> ([FromServices] IAntiforgery antiforgery,HttpContext context) =>
        {
            ILogger logger = loggerFactory.CreateLogger("GetAntiforgeryToken");
            try
            {
                AntiforgeryTokenSet token = antiforgery.GetAndStoreTokens(context);
                return TypedResults.Ok(token);
            }
            catch ( Exception ex )
            {
                logger.LogError(ex, $"获取防伪令牌发生异常");
                return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
            }
        })
            .WithName("GetAntiforgeryToken")
            .WithDisplayName("获取防伪令牌接口")
            .WithSummary("获取防伪令牌接口")
            .WithDescription("获取防伪令牌接口，用于生成 Post 请求时需要的防伪令牌");
    }
}
