using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

using TMod.Blog.Application.Common.Options;
using TMod.Blog.Application.Common.Results;

namespace TMod.Blog.Application.Middlewares
{
    internal class PackResultMiddleware
    {
        private readonly RequestDelegate? _next;
        private readonly ILogger<PackResultMiddleware> _logger;

        public PackResultMiddleware(RequestDelegate next, ILogger<PackResultMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_next is null)
            {
                return;
            }

            // 跳过包装的标识（或非 API 路径）
            if (context.Request.Headers.ContainsKey("X-No-Result-Wrap") || !context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            var originalBody = context.Response.Body;
            await using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            try
            {
                await _next(context);

                memStream.Seek(0, SeekOrigin.Begin);

                var contentType = context.Response.ContentType ?? string.Empty;

                // 如果响应意图作为下载/附件，或明确包含 Content-Disposition，则跳过包装
                if (context.Response.Headers.ContainsKey("Content-Disposition"))
                {
                    memStream.Seek(0, SeekOrigin.Begin);
                    context.Response.Body = originalBody;
                    await memStream.CopyToAsync(originalBody);
                    return;
                }

                // 如果 Content-Type 明确是常见的文件类型（非 JSON），则跳过包装
                if (!contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    bool isFileLike = contentType.StartsWith("application/octet-stream", StringComparison.OrdinalIgnoreCase)
                        || contentType.StartsWith("application/pdf", StringComparison.OrdinalIgnoreCase)
                        || contentType.StartsWith("application/zip", StringComparison.OrdinalIgnoreCase)
                        || contentType.StartsWith("application/x-", StringComparison.OrdinalIgnoreCase)
                        || contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                        || contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase)
                        || contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase)
                        || contentType.StartsWith("text/csv", StringComparison.OrdinalIgnoreCase)
                        || contentType.StartsWith("application/vnd", StringComparison.OrdinalIgnoreCase);

                    if (isFileLike)
                    {
                        memStream.Seek(0, SeekOrigin.Begin);
                        context.Response.Body = originalBody;
                        await memStream.CopyToAsync(originalBody);
                        return;
                    }
                }

                // 读取原始响应体
                memStream.Seek(0, SeekOrigin.Begin);
                string raw = await new StreamReader(memStream, Encoding.UTF8).ReadToEndAsync();
                string trimmed = raw?.Trim() ?? string.Empty;

                // 如果没有内容并且状态码是 204 (No Content)，直接透传空响应
                if (string.IsNullOrWhiteSpace(trimmed) && context.Response.StatusCode == StatusCodes.Status204NoContent)
                {
                    context.Response.Body = originalBody;
                    return;
                }

                // 判断是否为 JSON：基于 content-type 或内容首字符
                bool contentTypeIsJson = contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase);
                bool looksLikeJson = !string.IsNullOrWhiteSpace(trimmed) && (trimmed.StartsWith("{") || trimmed.StartsWith("["));

                if (!contentTypeIsJson && !looksLikeJson)
                {
                    // 非 JSON 直接透传
                    memStream.Seek(0, SeekOrigin.Begin);
                    context.Response.Body = originalBody;
                    await memStream.CopyToAsync(originalBody);
                    return;
                }

                // 如果 body 为空但不是 204，则把 payload 视为 null 并进行包装（保留状态码）
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    var wrappedEmpty = Result.Ok(null);
                    string jsonEmpty = JsonSerializer.Serialize(wrappedEmpty, ApplicationJsonSerializerOptions.Default);
                    context.Response.ContentType = "application/json";
                    context.Response.Body = originalBody;
                    await context.Response.WriteAsync(jsonEmpty);
                    return;
                }

                // 解析 JSON
                JsonDocument? doc = null;
                try
                {
                    doc = JsonDocument.Parse(trimmed);
                }
                catch
                {
                    // 解析失败，直接透传原始内容
                    memStream.Seek(0, SeekOrigin.Begin);
                    context.Response.Body = originalBody;
                    await memStream.CopyToAsync(originalBody);
                    return;
                }

                var root = doc.RootElement;

                // 检测是否已经是 Result 结构（包含 isSuccess 属性，忽略大小写）
                bool alreadyWrapped = false;
                if (root.ValueKind == JsonValueKind.Object)
                {
                    foreach (var p in root.EnumerateObject())
                    {
                        if (string.Equals(p.Name, "isSuccess", StringComparison.OrdinalIgnoreCase))
                        {
                            alreadyWrapped = true;
                            break;
                        }
                    }
                }

                if (alreadyWrapped)
                {
                    // 已经包装过，直接返回原始 JSON
                    memStream.Seek(0, SeekOrigin.Begin);
                    context.Response.Body = originalBody;
                    await memStream.CopyToAsync(originalBody);
                    return;
                }

                // 未包装，根据状态码决定 OK/Fail
                Result wrappedResult;
                if (context.Response.StatusCode >= 400)
                {
                    wrappedResult = Result.Fail(root);
                }
                else
                {
                    wrappedResult = Result.Ok(root);
                }

                var output = JsonSerializer.Serialize(wrappedResult, ApplicationJsonSerializerOptions.Default);

                context.Response.ContentType = "application/json";
                context.Response.Body = originalBody;
                await context.Response.WriteAsync(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "包装 Result 结构体时发生异常");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                context.Response.Body = originalBody;
                await context.Response.WriteAsync(JsonSerializer.Serialize(Result.Fail(null, ex.Message), ApplicationJsonSerializerOptions.Default));
            }
        }
    }
}
