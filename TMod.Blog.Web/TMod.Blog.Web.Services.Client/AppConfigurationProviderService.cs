using System;
using System.Collections.Generic;
using System.Linq;
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

        public AppConfigurationProviderService(IAppConfigurationApi appConfigurationApi)
        {
            _appConfigurationApi = appConfigurationApi;
        }

        public Task<string?> GetConfigurationValueAsync(string key) => _appConfigurationApi.GetConfigurationValueAsync(key);

        public Task<T?> GetConfigurationValueAsync<T>(string key, JsonSerializerOptions? jsonSerializerOptions = null) => _appConfigurationApi.GetConfigurationValueAsync<T>(key, jsonSerializerOptions);
    }
}
