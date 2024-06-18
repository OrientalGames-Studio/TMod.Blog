using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Services.Client
{
    internal class LocalStorageProviderService : ILocalStorageProviderService
    {
        private readonly IJSRuntime _jsModule;
        private readonly ILogger<LocalStorageProviderService> _logger;
        public LocalStorageProviderService(IJSRuntime jsModule,ILogger<LocalStorageProviderService> logger)
        {
            _jsModule = jsModule;
            _logger = logger;
        }

        public async Task<string?> GetAsync(string key)
        {
            string? value;
            try
            {
                value = await _jsModule.InvokeAsync<string>("localStorage.getItem", key);
                _logger.LogInformation($"从 LocalStorage 中读取 {key} 的值是 {value}");
                if ( string.IsNullOrWhiteSpace(value) )
                {
                    return null;
                }
            }catch(Exception ex )
            {
                _logger.LogError(ex, $"访问 LocalStorage 时发生异常");
                return null;
            }
            try
            {
                JsonDocument document = JsonDocument.Parse(value);
                value = document.RootElement.GetString();
            }
            catch(JsonException ex )
            {
                _logger.LogWarning(ex, $"尝试把值 {value} 从 Json 转换为字符串时发生异常");
            }
            return value;
        }

        public async Task<T?> GetAsync<T>(string key,JsonSerializerOptions? options = null)
        {
            string? value = await GetAsync(key);
            if ( string.IsNullOrWhiteSpace(value) )
            {
                return default;
            }
            try
            {
                JsonDocument jsonDocument = JsonDocument.Parse(value);
                return jsonDocument.Deserialize<T>(options);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"将 LocalStorage 中 {key} 的值 {value} 反序列化为 {typeof(T)}? 时发生异常");
                return default;
            }
        }

        public async Task SetAsync(string key, object? value, JsonSerializerOptions? options = null)
        {
            try
            {
                if(value is null )
                {
                    value = "";
                }
                string? jsonValue = JsonSerializer.Serialize(value,options);
                await _jsModule.InvokeVoidAsync("localStorage.setItem",key,jsonValue);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"访问 LocalStorage 时发生异常");
            }
        }
    }
}
