using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

using TMod.Blog.Application.Common.Results;
using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Category;
using TMod.Blog.Application.Services;

namespace TMod.Blog.Api.Endpoints
{
    internal static class CategoryEndpoints
    {
        private static ILogger? _logger;
        public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/categories")
                .RequireCors("blog")
                .RequireRateLimiting("default")
                .WithDescription("分类接口")
                .WithGroupName("v1")
                .WithSummary("博客的分类接口")
                .WithTags("categories")
                .ProducesProblem(StatusCodes.Status429TooManyRequests)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
            ILoggerProvider loggerProvider = app.ServiceProvider.GetRequiredService<ILoggerProvider>();
            _logger = loggerProvider.CreateLogger("TMod.Blog.Api.Article");
            group.MapPost("/", CreateCategoryAsync)
                .WithName("CreateCategory")
                .WithSummary("创建分类")
                .WithDescription("这个接口会创建一条 Category 数据")
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            group.MapGet("/{categoryId:guid}", GetCategoryByIdAsync)
                .WithName("GetCategoryById")
                .WithSummary("根据Id获取分类")
                .WithDescription("这个接口会根据传入的Id获取一个分类信息")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            group.MapGet("/", PagingCategoryAsync)
                .WithName("PagingCategory")
                .WithSummary("分页查询分类")
                .WithDescription("这个接口可以分页查询和筛选分类列表")
                .Produces(StatusCodes.Status200OK);
            group.MapDelete("/{categoryId:guid}", DeleteCategoryAsync)
                .WithName("DeleteCategory")
                .WithSummary("删除分类")
                .WithDescription("这个接口可以删除分类数据，如果有子分类会导致删除失败")
                .Produces(StatusCodes.Status204NoContent);
            group.MapPatch("/{categoryId:guid}", PatchCategoryParentAsync)
                .WithName("ChangeCategoryParent")
                .WithSummary("修改分类父分类")
                .WithDescription("这个接口可以修改一个分类的父分类")
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            group.MapPut("/{categoryId:guid}", PutCategoryAsync)
                .WithName("UpdateCategory")
                .WithSummary("更新分类")
                .WithDescription("这个接口可以全量幂等的更新分类")
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            group.MapHealthChecks("categories-api-health");
            return app;
        }

        private static async Task<Results<CreatedAtRoute<CategoryDto>,ValidationProblem,StatusCodeHttpResult>> CreateCategoryAsync([FromBody]CreateCategoryRequest request,IValidator<CreateCategoryRequest> validator,ICategoryService categoryService,CancellationToken token)
        {
            var validationResults = await validator.ValidateAsync(request,token);
            if ( !validationResults.IsValid )
            {
                return TypedResults.ValidationProblem(validationResults.ToDictionary());
            }
            try
            {
                var category = await categoryService.CreateCategoryAsync(request,token);
                if(category is null )
                {
                    _logger?.LogCritical("因为不明原因导致分类创建返回空值");
                    return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
                }
                return TypedResults.CreatedAtRoute(category, "GetCategoryById", new { categoryId = category.Id });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "新增分类时发生错误");
                throw;
            }
        }

        private static async Task<Results<JsonHttpResult<CategoryDto>,NotFound<string>,StatusCodeHttpResult>> GetCategoryByIdAsync([FromRoute]Guid categoryId,ICategoryService categoryService,CancellationToken token)
        {
            try
            {
                var category = await categoryService.GetCategoryByIdAsync(categoryId,token);
                if(category is null )
                {
                    return TypedResults.NotFound("分类不存在");
                }
                return TypedResults.Json(category);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "加载分类发生错误");
                throw;
            }
        }

        private static async Task<Results<JsonHttpResult<Result>,StatusCodeHttpResult>> PagingCategoryAsync([FromServices] ICategoryService categoryService,[FromQuery]Guid? parentId = null, [FromQuery]string? categoryName = null,[FromQuery]int pageIndex = 1,[FromQuery]int pageSize = 20,CancellationToken token = default)
        {
            try
            {
                var categoryList = await categoryService.PagingCategoriesByParentIdAsync(parentId,categoryName,pageIndex,pageSize,token);
                return TypedResults.Json(( Result )categoryList);
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "分页查询分类发生错误");
                //return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        private static async Task<Results<NoContent,NotFound<string>,StatusCodeHttpResult>> DeleteCategoryAsync([FromRoute]Guid categoryId,ICategoryService categoryService,CancellationToken token)
        {
            try
            {
                bool isDeleted = await categoryService.DeleteCategoryAsync(categoryId,token);
                if ( isDeleted )
                {
                    return TypedResults.NoContent();
                }
                return TypedResults.NotFound("删除分类失败");
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "删除分类发生错误");
                throw;
            }
        }

        private static async Task<Results<CreatedAtRoute<CategoryDto>,ValidationProblem,BadRequest<string>,StatusCodeHttpResult>> PatchCategoryParentAsync([FromRoute]Guid categoryId, [FromBody]PatchCategoryParentRequest request,IValidator<PatchCategoryParentRequest> validator,ICategoryService categoryService,CancellationToken token)
        {
            var validationResult = await validator.ValidateAsync(request,token);
            if ( !validationResult.IsValid )
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }
            try
            {
                var category = await categoryService.ChangeParentCategoryAsync(categoryId,request,token);
                if(category is null )
                {
                    return TypedResults.BadRequest("修改分类的父分类时因为不明原因返回空值");
                }
                return TypedResults.CreatedAtRoute(category, "GetCategoryById", new { categoryId = category.Id });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "修改分类的父分类发生错误");
                throw;
            }
        }

        private static async Task<Results<CreatedAtRoute<CategoryDto>,ValidationProblem, BadRequest<string>>> PutCategoryAsync([FromRoute]Guid categoryId, [FromBody]UpdateCategoryRequest request,IValidator<UpdateCategoryRequest> validator,ICategoryService categoryService,CancellationToken token)
        {
            var validationResults = await validator.ValidateAsync(request,token);
            if ( !validationResults.IsValid )
            {
                return TypedResults.ValidationProblem(validationResults.ToDictionary());
            }
            try
            {
                var category = await categoryService.UpdateCategoryAsync(categoryId,request,token);
                if(category is null )
                {
                    return TypedResults.BadRequest("更新分类失败");
                }
                return TypedResults.CreatedAtRoute(category, "GetCategoryById", new { categoryId = category.Id });
            }
            catch ( Exception ex )
            {
                _logger?.LogCritical(ex, "更新分类发生错误");
                throw;
            }
        }
    }
}
