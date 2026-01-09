using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.SiteConfiguration;

namespace TMod.Blog.Application.Services
{
    public interface ISiteConfigurationService
    {
        Task<SiteConfigurationDto> AddConfigurationAsync(CreateConfigurationRequest request, CancellationToken token = default);

        Task<SiteConfigurationDto?> UpdateConfigurationAsync(int configId,UpdateConfigurationRequest request, CancellationToken token = default);

        Task<SiteConfigurationDto?> UpdateConfigurationEnabledAsync(int configId,PatchConfigurationRequest request, CancellationToken token = default);

        Task<SiteConfigurationDto?> UpdateConfigurationValueByKeyAsync(string configKey,string? configValue,CancellationToken token = default);

        Task<SiteConfigurationDto?> GetConfigurationAsync(string configKey, CancellationToken token = default);

        Task<SiteConfigurationDto?> GetConfigurationByIdAsync(int configId, CancellationToken token = default);

        Task<PagingDto<SiteConfigurationDto>> PagingConfigurationsAsync(string? keyword = null, int pageIndex = 1, int pageSize = 20, CancellationToken token = default);

        Task<bool> DeleteConfigurationAsync(string configKey,CancellationToken token = default);

        Task<bool> DeleteConfigurationByIdAsync(int configId,CancellationToken token = default);
    }
}
