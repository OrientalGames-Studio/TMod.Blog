using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.StoreServices;
using TMod.Blog.Data.Models;
using TMod.Blog.Data.Models.ViewModels.Configuration;

namespace TMod.Blog.Data.Services
{
    public interface IConfigurationStoreService:IStoreService,IPagingStoreService<ConfigurationViewModel>
    {
        IAsyncEnumerable<ConfigurationViewModel> GetAllConfigurationsAsync();

        Task<ConfigurationViewModel?> LoadConfigurationAsync(int configurationId);

        Task<ConfigurationViewModel?> LoadConfigurationAsync(string configurationKey);

        Task<ConfigurationViewModel> CreateConfigurationAsync(string configurationKey,object? value);

        Task<ConfigurationViewModel?> UpdateConfigurationAsync(int configurationId, object? value);

        Task<ConfigurationViewModel?> UpdateConfigurationAsync(string configurationKey,object? value);

        Task RemoveConfigurationAsync(int configurationId);

        Task RemoveConfigurationAsync(string configurationKey);

        Task BatchRemoveConfigurationAsync(params int[] configurationIds);
    }
}
