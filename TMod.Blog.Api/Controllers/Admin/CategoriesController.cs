using Microsoft.AspNetCore.Mvc;

using TMod.Blog.Data.Models.DTO.Categories;
using TMod.Blog.Data.Models.ViewModels.Categories;
using TMod.Blog.Data.Services;

namespace TMod.Blog.Api.Controllers.Admin
{
    [Route("api/v1/admin/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly ICategoryStoreService _categoryStoreService;

        public CategoriesController(ILogger<CategoriesController> logger, ICategoryStoreService categoryStoreService)
        {
            _logger = logger;
            _categoryStoreService = categoryStoreService;
        }

        [HttpGet]
        public IActionResult GetAllCategoriesAsync([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, [FromQuery] string? categoryFilter = null, [FromQuery] DateOnly? createDateFrom = null, [FromQuery] DateOnly? createDateTo = null)
        {
            object pagingResult;
            try
            {
                //IEnumerable<CategoryViewModel?> categories = _categoryStoreService.Paging(pageIndex,pageSize,out int totalDataCount,out int totalPageCount,p=>string.IsNullOrWhiteSpace(categoryFilter)?true:(p?.Category.Contains(categoryFilter) == true));
                IEnumerable<CategoryViewModel?> categories = _categoryStoreService.GetAllCategoriesAsync().ToBlockingEnumerable();
                if ( !string.IsNullOrWhiteSpace(categoryFilter) )
                {
                    categories = categories.Where(p => p?.Category.Contains(categoryFilter) == true);
                }
                if ( createDateFrom is not null && createDateTo is not null )
                {
                    categories = categories.Where(p => p is not null && ( DateOnly.FromDateTime(p.CreateDate) >= createDateFrom ) && ( DateOnly.FromDateTime(p.CreateDate) < createDateTo ));
                }
                else if ( createDateFrom is not null )
                {
                    categories = categories.Where(p => p is not null && DateOnly.FromDateTime(p.CreateDate) >= createDateFrom);
                }
                else if ( createDateTo is not null )
                {
                    categories = categories.Where(p => p is not null && DateOnly.FromDateTime(p.CreateDate) < createDateTo);
                }
                IQueryable<CategoryViewModel?> viewModels = _categoryStoreService.Paging(categories,pageIndex,pageSize,out int totalDataCount,out int totalPageCount);
                pagingResult = new
                {
                    pageIndex = pageIndex,
                    pageSize = pageSize,
                    dataCount = totalDataCount,
                    pageCount = totalPageCount,
                    data = viewModels
                };
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"分页查询分类项时发生异常，当前页码：{pageIndex}，当前单页数据量：{pageSize}");
                pagingResult = new
                {
                    pageIndex = pageIndex,
                    pageSize = pageSize,
                    dataCount = 0,
                    pageCount = 1,
                    data = new List<CategoryViewModel>()
                };
            }
            return Ok(pagingResult);
        }

        [ActionName(nameof(GetCategoryByIdAsync))]
        [HttpGet("id/{id:int}", Name = nameof(GetCategoryByIdAsync))]
        public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] int id)
        {
            CategoryViewModel? vm = await _categoryStoreService.LoadCategoryAsync(id);
            if ( vm is null )
            {
                return NotFound();
            }
            return Ok(vm);
        }

        [HttpGet("{category}", Name = nameof(GetCategoryByNameAsync))]
        public async Task<IActionResult> GetCategoryByNameAsync([FromRoute] string category)
        {
            CategoryViewModel? vm = await _categoryStoreService.LoadCategoryAsync(category);
            if ( vm is null )
            {
                return NotFound();
            }
            return Ok(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] AddCategoryModel model)
        {
            try
            {
                if ( !ModelState.IsValid )
                {
                    return BadRequest(ModelState);
                }
                CategoryViewModel viewModel = await _categoryStoreService.CreateCategoryAsync(model.Category);
                return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = viewModel.Id }, viewModel);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"在添加分类项:{model.Category}时发生异常");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("id/{id:int}", Name = nameof(RemoveCategoryByIdAsync))]
        public async Task<IActionResult> RemoveCategoryByIdAsync([FromRoute] int id)
        {
            try
            {
                await _categoryStoreService.RemoveCategoryAsync(id);
                return NoContent();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"根据编号{id}删除分类项时发生异常");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{category}", Name = nameof(RemoveCategoryByNameAsync))]
        public async Task<IActionResult> RemoveCategoryByNameAsync([FromRoute] string category)
        {
            try
            {
                await _categoryStoreService.RemoveCategoryAsync(category);
                return NoContent();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"根据分类名称{category}删除分类项时发生异常");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> BatchRemoveCategoryByIdAsync([FromBody]BatchDeleteCategoryModel? model)
        {
            if ( !ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _categoryStoreService.BatchRemoveCategoryByIdAsync(model!.CategoryIds?.ToArray() ?? []);
                return NoContent();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"批量删除分类时发生异常，分类编号:({string.Join(",", model?.CategoryIds?.ToArray() ?? [])})");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPatch("id/{id:int}", Name = nameof(UpdateCategoryByIdAsync))]
        public async Task<IActionResult> UpdateCategoryByIdAsync([FromRoute] int id, [FromBody] UpdateCategoryModel model)
        {
            if ( !ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }
            try
            {
                CategoryViewModel? viewModel = await _categoryStoreService.UpdateCategoryAsync(id,model.Category!);
                if ( viewModel is null )
                {
                    return NotFound();
                }
                return Ok(viewModel);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"根据编号{id}修改分类项时发生异常");
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{category}", Name = nameof(UpdateCategoryByNameAsync))]
        public async Task<IActionResult> UpdateCategoryByNameAsync([FromRoute] string category, [FromBody] UpdateCategoryModel model)
        {
            if ( !ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }
            try
            {
                CategoryViewModel? viewModel = await _categoryStoreService.UpdateCategoryAsync(category, model.Category!);
                if ( viewModel is null )
                {
                    return NotFound();
                }
                return Ok(viewModel);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"根据分类名称{category}修改分类时发生异常");
                return BadRequest(ex.Message);
            }
        }
    }
}
