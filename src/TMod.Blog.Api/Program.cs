using Microsoft.EntityFrameworkCore;

using TMod.Blog.Infrastructure.Contextes;
using TMod.Blog.Api.Endpoints;
using TMod.Blog.Application;
using TMod.Blog.Infrastructure;
using Microsoft.Extensions.Caching.Hybrid;
using TMod.Blog.Api.Extensions;
using System.Threading.RateLimiting;
using TMod.Blog.Infrastructure.CompiledModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Services.AddOpenApi("v1");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    //options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContextPool<TMod_Blog_RW_Context>((provider, options) =>
{
    ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    IMemoryCache memoryCache = provider.GetRequiredService<IMemoryCache>();
    options.UseSqlServer(builder.Configuration.GetConnectionString("TMod.Blog_RW"))
    .UseModel(TMod_Blog_RW_ContextModel.Instance)
    .UseLoggerFactory(loggerFactory)
    .UseMemoryCache(memoryCache);
    IHostEnvironment hostEnvironment = provider.GetRequiredService<IHostEnvironment>();
    if ( hostEnvironment.IsDevelopment() )
    {
        options.EnableSensitiveDataLogging()
        .EnableDetailedErrors();
    }
});

builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions()
    {
        Expiration = TimeSpan.FromMinutes(30),
        LocalCacheExpiration = TimeSpan.FromSeconds(30)
    };
});
builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});

builder.Services.AddCors(options =>
{
    options.DefaultPolicyName = "default";
    options.AddDefaultPolicy(policy =>
    {
        policy.WithMethods(HttpMethods.Get)
        .SetIsOriginAllowed(_ => true)
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyHeader()
        .AllowCredentials();
    });
    options.AddPolicy("blog", policy =>
    {
        policy.WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Patch, HttpMethods.Delete)
        .WithOrigins("127.0.0.1", "localhost", "orientalgames.cn")
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancel) =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        var ip = context.HttpContext.GetClientIp();
        var path = context.HttpContext.Request.Path;
        var traceId = Guid.NewGuid().ToString("N");
        logger.LogWarning("限流警告：客户端{}触发限流：（追踪ID: {TraceId} | IP: {IP} | 请求方法: {HttpMethod} | 路径: {Path} | 策略: {Policy}）", ip, traceId, ip,context.HttpContext.Request.Method, path, context.Lease.TryGetMetadata(MetadataName.ReasonPhrase, out string? policyName) ? policyName : "Unknow policy");

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(TMod.Blog.Application.Common.Results.Result.Fail(new
        {
            reference_id = traceId
        },"请求过于频繁；为了安全起见，您现在无法访问系统。请稍后再试。"),cancel);
    };
    options.AddPolicy("default-rate-limit-policy", httpcontext => RateLimitPartition.GetFixedWindowLimiter(httpcontext.GetClientIp(), _ =>new FixedWindowRateLimiterOptions()
    {
        PermitLimit = 5,
        Window = TimeSpan.FromMinutes(1)
    }));

    options.AddPolicy("article-rate-limit-policy", httpcontext => RateLimitPartition.GetTokenBucketLimiter(httpcontext.GetClientIp(), _ =>new TokenBucketRateLimiterOptions
    {
        TokenLimit = 10,
        ReplenishmentPeriod = TimeSpan.FromSeconds(20),
        TokensPerPeriod = 5,
        QueueLimit = 0
    }));



    options.AddPolicy("default", httpcontext => RateLimitPartition.GetTokenBucketLimiter(httpcontext.GetClientIp(), _ => new TokenBucketRateLimiterOptions
    {
        TokenLimit = 10,
        ReplenishmentPeriod = TimeSpan.FromSeconds(20),
        TokensPerPeriod = 5,
        QueueLimit = 0
    }));

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext => RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions()
    {
        PermitLimit = 100,
        Window = TimeSpan.FromSeconds(1)
    }));
});

builder.Services.AddHealthChecks();
builder.Services.AddBlogInfrastructure();
builder.Services.AddBlogApplication();


var app = builder.Build();
app.UseCors("default")
    .UseRateLimiter()
    .UseAntiforgery()
    .UseApplicationMiddleware();
app .MapAntiforgeryEndpoints()
    .MapArticleEndpoints()
    .MapCategoryEndpoints()
    .MapOpenApi();
app.Run();
