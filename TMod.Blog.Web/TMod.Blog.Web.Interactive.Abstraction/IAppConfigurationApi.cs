using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TMod.Blog.Web.Interactive.Abstraction
{
    public interface IAppConfigurationApi
    {
        Task<string?> GetConfigurationValueAsync(string key);

        Task<T?> GetConfigurationValueAsync<T>(string key, JsonSerializerOptions? jsonSerializerOptions = null);

        Task SetConfigurationValueAsync(string key,object? value);
    }
}
