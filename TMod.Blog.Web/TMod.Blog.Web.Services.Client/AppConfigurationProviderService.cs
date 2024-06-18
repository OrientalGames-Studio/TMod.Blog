using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TMod.Blog.Web.Interactive.Abstraction;
using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Services.Client
{
    internal class AppConfigurationProviderService : IAppConfigurationProviderService
    {
        private readonly IAppConfigurationApi _appConfigurationApi;
        private readonly ILocalStorageProviderService _localStorageProviderService;
        private readonly ILogger<AppConfigurationProviderService> _logger;
        private static readonly ConcurrentDictionary<string,List<ComponentBase>> _subscribeMaps = new ConcurrentDictionary<string, List<ComponentBase>>();
        private static readonly ConcurrentDictionary<string,object?> _configurationHistory = new ConcurrentDictionary<string, object?>();
        private static readonly MethodInfo? _componentStateHasChanged = typeof(ComponentBase).GetMethod("StateHasChanged",BindingFlags.NonPublic|BindingFlags.Instance);

        public AppConfigurationProviderService(IAppConfigurationApi appConfigurationApi,ILocalStorageProviderService localStorageProviderService,ILogger<AppConfigurationProviderService> logger)
        {
            _appConfigurationApi = appConfigurationApi;
            _localStorageProviderService = localStorageProviderService;
            _logger = logger;
        }

        public Task<string?> GetConfigurationValueAsync(string key) => _appConfigurationApi.GetConfigurationValueAsync(key);

        public Task<T?> GetConfigurationValueAsync<T>(string key, JsonSerializerOptions? jsonSerializerOptions = null) => _appConfigurationApi.GetConfigurationValueAsync<T>(key, jsonSerializerOptions);

        public async Task<string?> GetOrStoreConfigurationFromLocalStorage(string key)
        {
            string? localValue = null;
            bool needStore = false;
            try
            {
                localValue = await _localStorageProviderService.GetAsync(key);
            }
            catch ( Exception ex)
            {
                _logger.LogWarning(ex, $"从本地读取{key}的值发生异常，即将尝试从服务读取");
            }
            if ( string.IsNullOrWhiteSpace(localValue) )
            {
                needStore = true;
                localValue = await GetConfigurationValueAsync(key);
                _logger.LogInformation($"从服务读取 {key} 的值为 {localValue}");
            }
            if ( string.IsNullOrWhiteSpace(localValue) )
            {
                return null;
            }
            if ( needStore )
            {
                await SetAndStoreConfigurationValueAsync(key, localValue);
            }
            return localValue;
        }

        public async Task<T?> GetOrStoreConfigurationFromLocalStorage<T>(string key, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            T? localValue = default;
            bool needStore = false;
            try
            {
                localValue = await _localStorageProviderService.GetAsync<T>(key,jsonSerializerOptions);
            }
            catch ( Exception ex )
            {
                _logger.LogWarning(ex, $"从本地读取{key}的值发生异常，即将尝试从服务读取");
            }
            if ( localValue is null )
            {
                needStore = true;
                localValue = await GetConfigurationValueAsync<T>(key,jsonSerializerOptions);
                _logger.LogInformation($"从服务读取 {key} 的值为 {localValue}");
            }
            if ( localValue is null )
            {
                return default;
            }
            if ( needStore )
            {
                await SetAndStoreConfigurationValueAsync(key,localValue,jsonSerializerOptions);
            }
            return localValue;
        }

        public async Task SetAndStoreConfigurationValueAsync(string key, object? value, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            string? jsonValue;
            try
            {
                jsonValue = JsonSerializer.Serialize(value,jsonSerializerOptions);
            }
            catch ( JsonException ex )
            {
                _logger.LogError(ex, $"将 {key} 的值 {value} 序列化为 Json 发生异常");
                return;
            }
            try
            {
                await _localStorageProviderService.SetAsync(key, jsonValue);
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"将 {key} 的值 {jsonValue} 写入本地发生异常");
            }
        }

        public async Task SetConfigurationValueAsync(string key, object? value)
        {
            await _appConfigurationApi.SetConfigurationValueAsync(key, value);
        }
    }
}
