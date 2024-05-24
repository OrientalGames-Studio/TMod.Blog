using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using TMod.Blog.Data.Abstractions.StoreServices;
using TMod.Blog.Data.Models.DTO.Configuration;
using TMod.Blog.Data.Models.ViewModels.Configuration;
using TMod.Blog.Data.Services;

namespace TMod.Blog.Api.Controllers.Admin
{
    [Route("api/v1/admin/[controller]")]
    [ApiController]
    public class ConfigurationsController : ControllerBase
    {
        private readonly ILogger<ConfigurationsController> _logger;
        private readonly IConfigurationStoreService _configurationStoreService;

        public ConfigurationsController(ILogger<ConfigurationsController> logger, IConfigurationStoreService configurationStoreService)
        {
            _logger = logger;
            _configurationStoreService = configurationStoreService;
        }

        [HttpGet]
        public IActionResult GetAllConfigurations([FromQuery]int pageIndex = 1, [FromQuery]int pageSize = 20, [FromQuery]string? configKeyFilter = null)
        {
            object pagingResult;
            try
            {
                IQueryable<ConfigurationViewModel?> viewModels = _configurationStoreService.Paging(pageIndex,pageSize,out int totalDataCount,out int totalPageCount,vm=>string.IsNullOrWhiteSpace(configKeyFilter)?true:(vm?.Key.Contains(configKeyFilter) == true));
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
                _logger.LogError(ex, $"分页查询配置项时发生异常，当前单页数据量:{pageSize}，当前页码:{pageIndex}");
                pagingResult = new
                {
                    pageIndex = pageIndex,
                    pageSize = pageSize,
                    dataCount = 0,
                    pageCount = 1,
                    data = new List<ConfigurationViewModel>()
                };
            }
            return Ok(pagingResult);
        }

        [HttpGet("id/{configId:int}", Name = "GetConfigById")]
        public async Task<IActionResult> GetConfigurationByIdAsnyc([FromRoute]int configId)
        {
            ConfigurationViewModel? viewModel = await _configurationStoreService.LoadConfigurationAsync(configId);
            if ( viewModel is null )
            {
                return NotFound(configId);
            }
            return Ok(viewModel);
        }

        [HttpGet("{configKey}", Name = "GetConfigByKey")]
        public async Task<IActionResult> GetConfigurationByKeyAsync([FromRoute]string configKey)
        {
            ConfigurationViewModel? viewModel = await _configurationStoreService.LoadConfigurationAsync(configKey);
            if ( viewModel is null )
            {
                return NotFound(configKey);
            }
            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddConfigurationAsync([FromBody]AddConfigurationModel model)
        {
            try
            {
                if (! ModelState.IsValid )
                {
                    return BadRequest(ModelState);
                }
                ConfigurationViewModel vm = await _configurationStoreService.CreateConfigurationAsync(model.ConfigurationKey, model.ConfigurationValue);
                return CreatedAtAction("GetConfigById", vm.Id);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"尝试添加配置项:{model.ConfigurationKey} = {model.ConfigurationValue} 时发生异常");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("id/{configId:int}", Name = "RemoveConfigById")]
        public async Task<IActionResult> RemoveConfigurationByIdAsync([FromRoute] int configId)
        {
            try
            {
                await _configurationStoreService.RemoveConfigurationAsync(configId);
                return NoContent();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"尝试通过Id:{configId}删除配置项时发生异常");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{configKey}",Name = "RemoveConfigByKey")]
        public async Task<IActionResult> RemoveConfigurationByKeyAsync([FromRoute] string configKey)
        {
            try
            {
                await _configurationStoreService.RemoveConfigurationAsync(configKey);
                return NoContent();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"尝试通过key:{configKey}删除配置项时发生异常");
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("id/{configId:int}",Name = "UpdateConfigById")]
        public async Task<IActionResult> UpdateConfigurationByIdAsync([FromRoute]int configId, [FromBody]UpdateConfigurationModel model)
        {
            try
            {
                ConfigurationViewModel? vm = await _configurationStoreService.UpdateConfigurationAsync(configId, model.ConfigurationValue);
                return Ok(vm);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"根据编号{configId}修改配置项发生异常");
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{configKey}",Name = "UpdateConfigByKey")]
        public async Task<IActionResult> UpdateConfigurationByKeyAsync([FromRoute]string configKey, [FromBody]UpdateConfigurationModel model)
        {
            try
            {
                ConfigurationViewModel? vm = await _configurationStoreService.UpdateConfigurationAsync(configKey, model.ConfigurationValue);
                return Ok(vm);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"根据配置键{configKey}修改配置项发生异常");
                return BadRequest(ex.Message);
            }
        }
    }
}
