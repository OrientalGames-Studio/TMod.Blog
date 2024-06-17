using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TMod.Blog.Web.Services.Abstraction
{
    public interface IAppConfigurationProviderService
    {
        Task<string?> GetConfigurationValueAsync(string key);

        Task<T?> GetConfigurationValueAsync<T>(string key, JsonSerializerOptions? jsonSerializerOptions = null);
    }
}
