using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Configuration;
using TMod.Blog.Web.Models;

namespace TMod.Blog.Web.Interactive.Abstraction
{
    public interface IAppConfigurationApi
    {
        Task<string?> GetConfigurationValueAsync(string key);

        Task<T?> GetConfigurationValueAsync<T>(string key, JsonSerializerOptions? jsonSerializerOptions = null);

        Task SetConfigurationValueAsync(string key,object? value);

        Task<PagingResult<ConfigurationViewModel?>> GetAllConfigurations(int pageSize, int pageIndex = 1, string? configurationKeyFilter = null, DateOnly? createDateFrom = null, DateOnly? createDateTo = null);
    }
}
