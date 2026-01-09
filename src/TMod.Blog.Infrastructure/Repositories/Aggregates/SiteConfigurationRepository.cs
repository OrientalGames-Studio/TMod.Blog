using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Domain.Specifications;
using TMod.Blog.Infrastructure.Contextes;
using TMod.Blog.Infrastructure.Specifications;

namespace TMod.Blog.Infrastructure.Repositories.Aggregates
{
    internal class SiteConfigurationRepository(TMod_Blog_RW_Context context) : BlogRepository<SiteConfiguration, int>(context), ISiteConfigurationRepository
    {
        public async Task<SiteConfiguration?> GetConfigurationAsync(string configKey,bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            ISpecification<SiteConfiguration> specification = SiteConfigurationSpecification.CreateGetConfiguration(configKey);
            return await GetEntityAsync(specification, cancellationToken);
        }

        public async Task<IReadOnlyList<SiteConfiguration>> PagingConfigurationsAsync(string? keyword = null, bool showDisabled = false, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            int skip = (Math.Max(1,pageIndex) - 1) * pageSize;
            ISpecification<SiteConfiguration> specification = SiteConfigurationSpecification.CreatePaging(keyword,skip,pageSize,showDisabled);
            return await GetAllEntitiesAsync(specification, cancellationToken);
        }
    }
}
