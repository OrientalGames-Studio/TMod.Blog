using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TMod.Blog.Web.Services.Abstraction
{
    public interface ILocalStorageProviderService
    {
        Task<string?> GetAsync(string key);

        Task<T?> GetAsync<T>(string key,JsonSerializerOptions? options = null);

        Task SetAsync(string key, object? value, JsonSerializerOptions? options = null);
    }
}
