using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using TMod.Blog.Application.Common.Results;
using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.SiteConfiguration;
using TMod.Blog.Application.Services;

namespace TMod.Blog.Api.Endpoints
{
    internal static class SiteConfigurationEndpoints
    {
        private static ILogger? _logger;

        public static IEndpointRouteBuilder MapSiteConfigurationEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/configurations")
                .RequireCors("blog")
                .RequireRateLimiting("default")
                .WithDescription("配置接口")
                .WithGroupName("v1")
                .WithSummary("博客的配置接口")
                .WithTags("site-configurations")
                .ProducesProblem(StatusCodes.Status429TooManyRequests)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
            ILoggerProvider loggerProvider = app.ServiceProvider.GetRequiredService<ILoggerProvider>();
            _logger = loggerProvider.CreateLogger("TMod.Blog.Api.SiteConfiguration");

            group.MapPost("/", CreateConfigurationAsync)
                .WithName("AddConfiguration")
                .WithSummary("新增配置")
                .WithDescription("这个接口会新增一个配置")
                .Produces(StatusCodes.Status201Created)
                .ProducesValidationProblem();

            group.MapGet("/{configKey}", GetConfigurationAsync)
                .WithName("GetConfiguration")
                .WithSummary("获取配置")
                .WithDescription("这个接口可以根据配置键获取一个配置值")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPatch("/{configKey}", SetConfigurationIsEnabledAsync)
                .WithName("SetConfigurationEnabled")
                .WithSummary("设置配置是否启用")
                .WithDescription("这个接口可以设置配置是否启用")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPut("/{configKey}", UpdateConfigurationAsync)
                .WithName("UpdateConfiguration")
                .WithSummary("全量更新配置信息")
                .WithDescription("这个接口会全量的更新配置信息")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status404NotFound)
                .ProducesValidationProblem();

            group.MapGet("/", PagingConfigurationAsync)
                .WithName("PagingConfiguration")
                .WithSummary("获取所有配置")
                .WithDescription("这个接口可以查询所有的配置项")
                .Produces(StatusCodes.Status200OK);

            group.MapDelete("/{configKey}", DeleteConfigurationAsync)
                .WithName("DeleteConfiguration")
                .WithSummary("删除配置项")
                .WithDescription("这个接口可以删除配置项")
                .Produces(StatusCodes.Status204NoContent);
            return app;
        }

        private static async Task<Results<CreatedAtRoute<SiteConfigurationDto>, ValidationProblem, StatusCodeHttpResult>> CreateConfigurationAsync([FromBody]CreateConfigurationRequest request,IValidator<CreateConfigurationRequest> validator,ISiteConfigurationService siteConfigurationService,CancellationToken token)
        {
            var validationResult = await validator.ValidateAsync(request,token);
            if ( !validationResult.IsValid )
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }
            try
            {
                var configuration = await siteConfigurationService.AddConfigurationAsync(request,token);
                if(configuration is null )
                {
                    return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
                }
                return TypedResults.CreatedAtRoute(configuration, "GetConfiguration", new { configKey = configuration.ConfigKey });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "新增配置时发生错误");
                throw;
            }
        }

        private static async Task<Results<JsonHttpResult<SiteConfigurationDto>,NotFound,StatusCodeHttpResult>> GetConfigurationAsync([FromRoute]string configKey,ISiteConfigurationService siteConfigurationService,CancellationToken token)
        {
            try
            {
                var configuration = await siteConfigurationService.GetConfigurationAsync(configKey,token);
                if(configuration is null )
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.Json(configuration);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "获取配置发生错误");
                throw;
            }
        }

        private static async Task<Results<CreatedAtRoute<SiteConfigurationDto>,NotFound,StatusCodeHttpResult>> SetConfigurationIsEnabledAsync([FromRoute]string configKey, [FromBody]PatchConfigurationRequest request,ISiteConfigurationService siteConfigurationService,CancellationToken token)
        {
            try
            {
                var configuration = await siteConfigurationService.UpdateConfigurationEnabledAsync(configKey,request,token);
                if(configuration is null )
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.CreatedAtRoute(configuration, "GetConfiguration", new { configKey = configuration.ConfigKey });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "禁用/启用配置项发生错误");
                throw;
            }
        }

        private static async Task<Results<CreatedAtRoute<SiteConfigurationDto>,ValidationProblem,NotFound,StatusCodeHttpResult>> UpdateConfigurationAsync([FromRoute]string configKey, [FromBody]UpdateConfigurationRequest request,IValidator<UpdateConfigurationRequest> validator,ISiteConfigurationService siteConfigurationService,CancellationToken token)
        {
            var validationResult = await validator.ValidateAsync(request,token);
            if ( !validationResult.IsValid )
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }
            try
            {
                var configuration = await siteConfigurationService.UpdateConfigurationAsync(configKey,request,token);
                if(configuration is null )
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.CreatedAtRoute<SiteConfigurationDto>(configuration, "GetConfiguration", new { configKey = configuration.ConfigKey });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "修改配置时发生错误");
                throw;
            }
        }

        private static async Task<Results<JsonHttpResult<Result>,StatusCodeHttpResult>> PagingConfigurationAsync(ISiteConfigurationService siteConfigurationService,[FromQuery]string? keyword = null,CancellationToken token = default)
        {
            try
            {
                var configurations = await siteConfigurationService.GetAllConfigurationsAsync(keyword,token);
                return TypedResults.Json(( Result )configurations);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "查询所有配置项发生错误");
                throw;
            }
        }

        private static async Task<Results<NoContent,StatusCodeHttpResult>> DeleteConfigurationAsync([FromRoute]string configKey,ISiteConfigurationService siteConfigurationService,CancellationToken token)
        {
            try
            {
                await siteConfigurationService.DeleteConfigurationAsync(configKey, token);
                return TypedResults.NoContent();
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "删除配置项发生错误");
                throw;
            }
        }
    }
}
