using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

using TMod.Blog.Data.Models.ViewModels.Configuration;
using TMod.Blog.Web.Interactive.Abstraction;
using TMod.Blog.Web.Models;

namespace TMod.Blog.Web.Interactive
{
	internal class AppConfigurationApi : IAppConfigurationApi
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<AppConfigurationApi> _logger;

        public AppConfigurationApi(IHttpClientFactory httpClientFactory,ILogger<AppConfigurationApi> logger)
        {
            _apiClient = httpClientFactory.CreateClient("apiClient");
            _logger = logger;
        }

		public async Task<PagingResult<ConfigurationViewModel?>> GetAllConfigurations(int pageSize, int pageIndex = 1, string? configurationKeyFilter = null, DateOnly? createDateFrom = null, DateOnly? createDateTo = null)
		{
			try
			{
                string apiUrl = $"api/v1/admin/configurations?pageIndex={pageIndex}&pageSize={pageSize}{(string.IsNullOrWhiteSpace(configurationKeyFilter)?"":$"&configKeyFilter={HttpUtility.UrlEncode(configurationKeyFilter)}")}{(createDateFrom is null?"":$"&createDateFrom={HttpUtility.UrlEncode(createDateFrom.Value.ToString("yyyy-MM-dd"))}")}{(createDateTo is null?"":$"&createDateTo={HttpUtility.UrlEncode(createDateTo.Value.ToString("yyyy-MM-dd"))}")}";
				PagingResult<ConfigurationViewModel?>? datas = await _apiClient.GetFromJsonAsync<PagingResult<ConfigurationViewModel?>>(apiUrl);
                if(datas is null )
                {
                    return PagingResult<ConfigurationViewModel?>.Empty;
                }
                return datas;
			}
			catch ( Exception ex )
			{
				_logger.LogError(ex, $"请求接口获取所有配置项时发生异常");
				return PagingResult<ConfigurationViewModel?>.Empty;
			}
		}

		public async Task<string?> GetConfigurationValueAsync(string key)
        {
            try
            {
                string? configurationViewModelJson = await _apiClient.GetStringAsync($"api/v1/admin/configurations/{key}");
                JsonDocument configurationViewModel = JsonDocument.Parse(json:configurationViewModelJson);
                if(!configurationViewModel.RootElement.TryGetProperty("value",out JsonElement jsonElement) )
                {
                    return null;
                }
                return jsonElement.GetString();
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"读取配置[{key}]的值时发生异常");
                return null;
            }
        }

        public async Task<T?> GetConfigurationValueAsync<T>(string key, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            string? configurationValue = await GetConfigurationValueAsync(key);
            if ( string.IsNullOrWhiteSpace(configurationValue) )
            {
                return default;
            }
            try
            {
                JsonDocument jsonDocument = JsonDocument.Parse(configurationValue);
                return jsonDocument.Deserialize<T>(jsonSerializerOptions);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"将配置[{key}]的值{configurationValue}反序列化为{typeof(T)}?对象时发生异常");
                return default;
            }
        }

        public async Task SetConfigurationValueAsync(string key, object? value)
        {
            HttpResponseMessage patchResponse = await _apiClient.PatchAsJsonAsync($"api/v1/admin/configurations/{key}",new
            {
                ConfigurationValue = value
            });
            if ( patchResponse.IsSuccessStatusCode )
            {
                return;
            }
            if(patchResponse.StatusCode == System.Net.HttpStatusCode.NotFound )
            {
                patchResponse = await _apiClient.PostAsJsonAsync($"api/v1/admin/configurations", new
                {
                    ConfigurationKey = HttpUtility.UrlEncode(key),
                    ConfigurationValue = value
                });
                patchResponse.EnsureSuccessStatusCode();
            }
        }
    }
}
