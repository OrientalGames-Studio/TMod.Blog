using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.StoreServices;
using TMod.Blog.Data.Models;
using TMod.Blog.Data.Models.ViewModels.Configuration;
using TMod.Blog.Data.Repositories;

namespace TMod.Blog.Data.Services.Implements
{
    internal class ConfigurationStoreService : IConfigurationStoreService, IPagingStoreService<ConfigurationViewModel>
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILogger<ConfigurationStoreService> _logger;

        public ConfigurationStoreService(IConfigurationRepository configurationRepository, ILogger<ConfigurationStoreService> logger)
        {
            _configurationRepository = configurationRepository;
            _logger = logger;
        }

        public async Task<ConfigurationViewModel> CreateConfigurationAsync(string configurationKey, object? value)
        {
            Configuration? configuration = (await _configurationRepository.GetAllAsync()).FirstOrDefault(p=>p.Key.Equals(configurationKey));
            if(configuration is null )
            {
                configuration = new Configuration();
                configuration.Key = configurationKey;
                configuration.Value = JsonSerializer.Serialize(value);
                configuration.CreateMetaRecord();
                configuration = await _configurationRepository.CreateAsync(configuration);
            }
            else if ( configuration.IsRemove )
            {
                configuration.Key = $"{configuration.Key}_{configuration.Version}**{configuration.RemoveDate}";
                configuration.UpdateVersionRecord();
                _ = await _configurationRepository.UpdateAsync(configuration);
                configuration = new Configuration();
                configuration.Key = configurationKey;
                configuration.Value = JsonSerializer.Serialize(value);
                configuration.CreateMetaRecord();
                configuration = await _configurationRepository.CreateAsync(configuration);
            }
            return configuration!;
        }

        public async IAsyncEnumerable<ConfigurationViewModel> GetAllConfigurationsAsync()
        {
            IEnumerable<Configuration> configurations = (await _configurationRepository.GetAllAsync()).Where(p=>!p.IsRemove);
            foreach ( Configuration configuration in configurations )
            {
                ConfigurationViewModel? vm = configuration;
                vm ??= new ConfigurationViewModel()
                {
                    Id = configuration.Id,
                    Key = configuration.Key,
                    Value = configuration.Value
                };
                yield return vm;
            }
        }

        public async Task<ConfigurationViewModel?> LoadConfigurationAsync(int configurationId)
        {
            try
            {
                Configuration? configuration = await _configurationRepository.LoadAsync(configurationId);
                if ( configuration?.IsRemove == true )
                {
                    return null;
                }
                return configuration;
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"加载编号为 {configurationId} 的配置项发生异常");
                return null;
            }
        }

        public async Task<ConfigurationViewModel?> LoadConfigurationAsync(string configurationKey)
        {
            IEnumerable<Configuration> configurations = await _configurationRepository.GetAllAsync();
            Configuration? configuration = configurations.FirstOrDefault(p => p.Key.Equals(configurationKey));
            if(configuration?.IsRemove == true )
            {
                return null;
            }
            return configuration;
        }

        public IQueryable<ConfigurationViewModel?> Paging(int pageIndex, int pageSize, out int totalDataCount, out int totalPageCount,Func<ConfigurationViewModel?,bool>? filter = null)
        {
            Task<Task<IEnumerable<Configuration>>> getAllTask = new Task<Task<IEnumerable<Configuration>>>(async () => await _configurationRepository.GetAllAsync());
            getAllTask.RunSynchronously();
            IEnumerable<Configuration> configurations = getAllTask.Result.Result;
            return Paging(configurations, pageIndex, pageSize, out totalDataCount, out totalPageCount, filter);
        }

        public IQueryable<ConfigurationViewModel?> Paging(IEnumerable values, int pageIndex, int pageSize, out int totalDataCount, out int totalPageCount, Func<ConfigurationViewModel?, bool>? filter = null)
        {
            totalDataCount = 0;
            totalPageCount = 1;
            pageSize = Math.Max(1, pageSize);
            IEnumerable<Configuration>? configurations = values as IEnumerable<Configuration>;
            if(configurations is null )
            {
                configurations = [];
            }
            configurations = configurations.Where(p => !p.IsRemove);
            if ( filter is not null )
            {
                configurations = configurations.Where(p => filter.Invoke(p));
            }
            totalDataCount = configurations.Count();
            totalPageCount = Math.Max(1, ( int )Math.Ceiling(( double )totalDataCount / ( double )pageSize));
            pageIndex = Math.Max(1, Math.Min(pageIndex, totalPageCount));
            configurations = configurations.Skip(( pageIndex - 1 ) * pageSize).Take(pageSize);
            List<ConfigurationViewModel?> viewModels = new List<ConfigurationViewModel?>();
            foreach ( Configuration configuration in configurations )
            {
                viewModels.Add(configuration);
            }
            return viewModels.AsQueryable();
        }

        public async Task RemoveConfigurationAsync(int configurationId)
        {
            await _configurationRepository.RemoveAsync(configurationId);
        }

        public async Task RemoveConfigurationAsync(string configurationKey)
        {
            Configuration? configuration = (await _configurationRepository.GetAllAsync()).FirstOrDefault(p=>p.Key.Equals(configurationKey));
            if(configuration is not null )
            {
                await _configurationRepository.RemoveAsync(configuration);
            }
        }

        public async Task<ConfigurationViewModel?> UpdateConfigurationAsync(int configurationId, object? value)
        {
            Configuration? configuration = await _configurationRepository.LoadAsync(configurationId);
            if ( configuration?.IsRemove == true )
            {
                return null;
            }
            if(configuration is not null)
            {
                configuration.Value = JsonSerializer.Serialize(value);
                configuration.UpdateMetaRecord(true);
                configuration = await _configurationRepository.UpdateAsync(configuration);
            }
            return configuration;
        }

        public async Task<ConfigurationViewModel?> UpdateConfigurationAsync(string configurationKey, object? value)
        {
            Configuration? configuration = (await _configurationRepository.GetAllAsync()).FirstOrDefault(p=>p.Key.Equals(configurationKey));
            if ( configuration?.IsRemove == true )
            {
                return null;
            }
            if ( configuration is not null )
            {
                configuration.Value = JsonSerializer.Serialize(value);
                configuration.UpdateMetaRecord(true);
                configuration = await _configurationRepository.UpdateAsync(configuration);
            }
            return configuration;
        }
    }
}
