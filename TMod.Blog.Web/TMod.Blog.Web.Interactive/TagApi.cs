using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Web.Interactive.Abstraction;

namespace TMod.Blog.Web.Interactive
{
    internal class TagApi : ITagApi
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<TagApi> _logger;

        public TagApi(IHttpClientFactory httpClientFactory, ILogger<TagApi> logger)
        {
            _apiClient = httpClientFactory.CreateClient("apiClient");
            _logger = logger;
        }
        public async Task<IEnumerable<string?>> GetTagsAsync()
        {
            string apiUrl = $"api/v1/admin/tags";
            try
            {
                IEnumerable<string?>? result = await _apiClient.GetFromJsonAsync<IEnumerable<string?>>(apiUrl);
                return result ?? [];
            }
            catch ( Exception ex )
            {
                return [];
            }
        }
    }
}
