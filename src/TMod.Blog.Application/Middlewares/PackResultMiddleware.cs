using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TMod.Blog.Application.Common.Options;
using TMod.Blog.Application.Common.Results;

namespace TMod.Blog.Application.Middlewares
{
    internal class PackResultMiddleware
    {
        private readonly RequestDelegate? _next;
        private readonly ILogger<PackResultMiddleware> _logger;

        public PackResultMiddleware(RequestDelegate next,ILogger<PackResultMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(_next is null )
            {
                return;
            }
            // 跳过包装的标识
            if ( context.Request.Headers.ContainsKey("X-No-Result-Wrap") )
            {
                await _next(context);
                return;
            }
            var originalBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            try
            {
                await _next(context);

                memStream.Seek(0, SeekOrigin.Begin);
                var contentType = context.Response.ContentType ?? "";

                // 非 JSON 直接透传
                if ( !contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase) )
                {
                    await memStream.CopyToAsync(originalBody);
                    return;
                }

                // 读取原始 JSON 内容
                string raw = await new StreamReader(memStream).ReadToEndAsync();
                object? payload = string.IsNullOrWhiteSpace(raw)
                ? null
                : JsonSerializer.Deserialize<object>(raw, ApplicationJsonSerializerOptions.Default);

                var wrapped = Result.Ok(payload);
                var json = JsonSerializer.Serialize(wrapped, ApplicationJsonSerializerOptions.Default);

                context.Response.ContentType = "application/json";
                context.Response.Body = originalBody;
                await context.Response.WriteAsync(json);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, "包装 Result 结构体时发生异常");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(Result.Fail(null, ex.Message), ApplicationJsonSerializerOptions.Default));
            }
        }
    }
}
