using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

using TMod.Blog.Data.Models.ViewModels.Categories;
using TMod.Blog.Web.Interactive.Abstraction;
using TMod.Blog.Web.Models;

namespace TMod.Blog.Web.Interactive
{
    internal class CategoryApi : ICategoryApi
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoryApi> _logger;

        public CategoryApi(IHttpClientFactory httpClientFactory, ILogger<CategoryApi> logger)
        {
            _httpClient = httpClientFactory.CreateClient("apiClient");
            _logger = logger;
        }

        public async Task<CategoryViewModel?> CreateCategoryAsync(string category)
        {
            if ( string.IsNullOrWhiteSpace(category) )
            {
                return null;
            }
            try
            {
                CategoryViewModel? metaData = await GetCategoryByCategoryNameAsync(category);
                if(metaData is not null )
                {
                    _logger.LogWarning($"当前已存在名为[{category}]的分类");
                    return metaData;
                }
                string apiUrl = $"api/v1/admin/categories";
                HttpResponseMessage responseMessage = await _httpClient.PostAsync(apiUrl,new StringContent(JsonSerializer.Serialize(new
                {
                    Category = category
                }),Encoding.UTF8,MediaTypeHeaderValue.Parse("application/json")));
                responseMessage.EnsureSuccessStatusCode();
                string categoryJson = await responseMessage.Content.ReadAsStringAsync();
                return JsonDocument.Parse(categoryJson).Deserialize<CategoryViewModel>();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口创建名为[{category}]的分类时发生异常");
                return null;
            }
        }

        public async Task<PagingResult<CategoryViewModel?>> GetAllCategoriesAsync(int pageSize, int pageIndex = 1, string? categoryKeyFilter = null, DateOnly? createDateFrom = null, DateOnly? createDateTo = null)
        {
            try
            {
                string apiUrl = $"api/v1/admin/categories?pageIndex={pageIndex}&pageSize={pageSize}{(string.IsNullOrWhiteSpace(categoryKeyFilter)?"":$"&categoryFilter={HttpUtility.UrlEncode(categoryKeyFilter)}")}{(createDateFrom.HasValue?$"&createDateFrom={HttpUtility.UrlEncode(createDateFrom.Value.ToString("yyyy-MM-dd"))}":"")}{(createDateTo.HasValue?$"&createDateTo={HttpUtility.UrlEncode(createDateTo.Value.ToString("yyyy-MM-dd"))}":"")}";
                PagingResult<CategoryViewModel?>? result = await _httpClient.GetFromJsonAsync<PagingResult<CategoryViewModel?>>(apiUrl);
                if(result is null )
                {
                    return PagingResult<CategoryViewModel?>.Empty;
                }
                return result;
            }
            catch ( Exception ex)
            {
                _logger.LogError(ex, $"请求接口获取所有分类时发生异常");
                return PagingResult<CategoryViewModel?>.Empty;
            }
        }

        public async Task<CategoryViewModel?> GetCategoryByCategoryNameAsync(string? categoryName)
        {
            try
            {
                string apiUrl = $"api/v1/admin/categories/{categoryName}";
                CategoryViewModel? viewModel = await _httpClient.GetFromJsonAsync<CategoryViewModel?>(apiUrl);
                return viewModel;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口获取名为[{categoryName}]的分类时发生异常");
                return null;
            }
        }

        public async Task<CategoryViewModel?> SaveCategoryAsync(string? originCategory, string category)
        {
            if ( string.IsNullOrWhiteSpace(originCategory) )
            {
                return await CreateCategoryAsync(category);
            }
            CategoryViewModel? metaData = await GetCategoryByCategoryNameAsync(originCategory);
            if(metaData is null )
            {
                _logger.LogWarning($"修改名为[{originCategory}]的分类时，元数据不存在");
                return null;
            }
            return await UpdateCategoryAsync(metaData.Category, category);
        }

        public async Task<CategoryViewModel?> UpdateCategoryAsync(string originCategory, string category)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(originCategory);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(category);
            if(originCategory == category )
            {
                return await GetCategoryByCategoryNameAsync(originCategory);
            }
            try
            {
                string apiUrl = $"api/v1/admin/categories/{originCategory}";
                HttpResponseMessage response = await _httpClient.PatchAsJsonAsync(apiUrl,new
                {
                    Category = category,
                });
                response.EnsureSuccessStatusCode();
                string categoryJson = await response.Content.ReadAsStringAsync();
                return JsonDocument.Parse(categoryJson).Deserialize<CategoryViewModel>();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口将分类[{originCategory}]修改为[{category}]时发生异常");
                return null;
            }
        }

        public async Task BatchRemoveCategoryByIdAsync(params int[] categoryIds)
        {
            try
            {
                string apiUrl = $"api/v1/admin/categories";
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete,apiUrl);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    CategoryIds = categoryIds ?? []
                }),Encoding.UTF8,MediaTypeHeaderValue.Parse("application/json"));
                HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);
                responseMessage.EnsureSuccessStatusCode();
                return;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"请求接口批量删除分类时发生异常，分类 Id: ({string.Join(",",categoryIds)})");
                throw;
            }
        }
    }
}
