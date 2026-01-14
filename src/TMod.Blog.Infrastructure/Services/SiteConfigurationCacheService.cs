using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Domain.Interfaces.Services;

namespace TMod.Blog.Infrastructure.Services
{
    internal class SiteConfigurationCacheService : ISiteConfigurationCacheService
    {
        private readonly ISiteConfigurationRepository _siteConfigurationRepository;
        private readonly ILogger<SiteConfigurationCacheService> _logger;
        private readonly HybridCache _hybridCache;

        public SiteConfigurationCacheService(ISiteConfigurationRepository siteConfigurationRepository, ILogger<SiteConfigurationCacheService> logger,
          HybridCache hybridCache)
        {
            _siteConfigurationRepository = siteConfigurationRepository;
            _logger = logger;
            _hybridCache = hybridCache;
        }

        public string? GetConfiguration(string key)
        {
            //if(_cache.TryGetValue(key,out string? value) )
            //{
            //    return value;
            //}

            //SiteConfiguration? config = _siteConfigurationRepository.GetConfigurationAsync(key).GetAwaiter().GetResult();
            //if(config is not null && !string.IsNullOrWhiteSpace(config.ConfigValue) )
            //{
            //    value = config.ConfigValue;
            //    _cache.AddOrUpdate(key, config.ConfigValue, (key, value) => config.ConfigValue);
            //}
            //return value;
            return _hybridCache.GetOrCreateAsync<string?>(key, async cancel =>
            {
                var configuration = await _siteConfigurationRepository.GetConfigurationAsync(key,true,cancel);
                return configuration?.ConfigValue;
            }, tags: [key,"site-configurations"]).Result;
        }

        public void Refresh()
        {
            _hybridCache.RemoveByTagAsync("site-configurations");
            _logger.LogInformation("Site configuration cache refreshed.");
        }
    }
}
