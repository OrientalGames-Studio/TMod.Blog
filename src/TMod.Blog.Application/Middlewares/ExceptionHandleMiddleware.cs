using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TMod.Blog.Application.Common.Options;
using TMod.Blog.Application.Common.Results;

namespace TMod.Blog.Application.Middlewares
{
    internal class ExceptionHandleMiddleware
    {
        private readonly RequestDelegate? _next;
        private readonly ILogger<ExceptionHandleMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, int> _exceptionCounter = new();

        public ExceptionHandleMiddleware(RequestDelegate next,ILogger<ExceptionHandleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch ( Exception ex )
            {
                // 1️⃣ 记录异常出现次数
                var key = ex.GetType().FullName ?? "UnknownException";
                _exceptionCounter.AddOrUpdate(key, 1, (_, count) => count + 1);

                // 2️⃣ 记录日志详细信息
                _logger.LogError(ex,
                    """
                [ × ] 捕获未处理异常:
                Type: {ExceptionType}
                Message: {Message}
                Path: {Path}
                Time: {Time}
                Count: {Count}
                """,
                    key,
                    ex.Message,
                    context.Request.Path,
                    DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    _exceptionCounter[key]
                );

                // 3️⃣ 设置响应状态码
                context.Response.StatusCode = ( int )HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                // 4️⃣ 返回错误结果
                var result = Result.Fail($"服务器发生错误: {ex.Message}");
                await context.Response.WriteAsync(JsonSerializer.Serialize(result, ApplicationJsonSerializerOptions.Default));
            }
        }
    }
}
