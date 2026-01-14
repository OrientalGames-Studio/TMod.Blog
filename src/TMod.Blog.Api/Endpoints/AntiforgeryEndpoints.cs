using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

using TMod.Blog.Application.Common.Options;

namespace TMod.Blog.Api.Endpoints
{
    public static class AntiforgeryEndpoints
    {
        public static IEndpointRouteBuilder MapAntiforgeryEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/antiforgery")
                .RequireCors("default")
                .RequireRateLimiting("default-rate-limit-policy")
                .WithDescription("防伪造请求接口")
                .WithGroupName("v1")
                .WithSummary("防伪造请求接口")
                .WithTags("antiforgery");
            group.MapHealthChecks("antiforgery-api-health");
            group.MapGet("token",GetAntiforgeryTokenAsync)
                .WithName("GetAntiforgeryToken")
                .WithSummary("生成防伪造令牌接口")
                .WithDescription("生成防伪造令牌接口，客户端可以通过此接口获取防伪造令牌，用于后续的请求中")
                .Produces<Ok<string>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);
            return app;
        }

        private static async Task<Results<Ok<string>,BadRequest<string>>> GetAntiforgeryTokenAsync([FromServices]IHttpContextAccessor httpContextAccessor, [FromServices]IAntiforgery antiforgery)
        {
            HttpContext? httpContext = httpContextAccessor.HttpContext;
            if(httpContext is null )
            {
                return TypedResults.BadRequest("获取 Antiforgery Token 失败，不是有效的Http客户端");
            }
            var tokens = antiforgery.GetAndStoreTokens(httpContext);
            await Task.CompletedTask;
            return TypedResults.Ok(JsonSerializer.Serialize(new
            {
                token = tokens.RequestToken,
                header_name = tokens.HeaderName,
                form_field_name = tokens.FormFieldName
            }, ApplicationJsonSerializerOptions.Default));
        }
    }
}
