using Azure.Core;

using MapsterMapper;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.SiteConfiguration;
using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Infrastructure.Specifications;

namespace TMod.Blog.Application.Services.Implements
{
    internal class SiteConfigurationService : ISiteConfigurationService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SiteConfigurationService> _logger;
        private readonly ISiteConfigurationRepository _siteConfigurationRepository;
        private readonly HybridCache _hybridCache;

        public SiteConfigurationService(IMapper mapper, ILogger<SiteConfigurationService> logger, ISiteConfigurationRepository siteConfigurationRepository,HybridCache hybridCache)
        {
            _mapper = mapper;
            _logger = logger;
            _siteConfigurationRepository = siteConfigurationRepository;
            _hybridCache = hybridCache;
        }

        public async Task<SiteConfigurationDto> AddConfigurationAsync(CreateConfigurationRequest request, CancellationToken token = default)
        {
            SiteConfiguration? siteConfiguration = await _siteConfigurationRepository.GetConfigurationAsync(request.ConfigKey, true,token);
            if(siteConfiguration is not null && !siteConfiguration.IsDeleted)
            {
                throw new InvalidOperationException($"配置[{request.ConfigKey}]已经存在，不能重复添加");
            }
            SiteConfigurationDto dto = _mapper.Map<SiteConfigurationDto>(request);
            SiteConfiguration entity = _mapper.Map<SiteConfiguration>(dto);
            await _siteConfigurationRepository.AddAsync(entity, token);
            await _hybridCache.SetAsync(dto.ConfigKey, dto, tags: [dto.ConfigKey, "site-configurations"],cancellationToken: token);
            return _mapper.Map<SiteConfigurationDto>(entity);
        }

        public async Task<bool> DeleteConfigurationAsync(string configKey, CancellationToken token = default)
        {
            SiteConfiguration? siteConfiguration = await _siteConfigurationRepository.GetConfigurationAsync(configKey, false,token);
            if(siteConfiguration is null || siteConfiguration.IsDeleted )
            {
                return false;
            }
            _siteConfigurationRepository.Delete(siteConfiguration);
            await _siteConfigurationRepository.SaveChangesAsync(token);
            await _hybridCache.RemoveAsync(configKey, token);
            return true;
        }

        public async Task<bool> DeleteConfigurationByIdAsync(int configId, CancellationToken token = default)
        {
            SiteConfiguration? siteConfiguration = await _siteConfigurationRepository.GetEntityByIdAsync(configId,false,token);
            if (siteConfiguration is null || siteConfiguration.IsDeleted)
            {
                return false;
            }
            _siteConfigurationRepository.Delete(siteConfiguration);
            await _siteConfigurationRepository.SaveChangesAsync(token);
            await _hybridCache.RemoveAsync(siteConfiguration.ConfigKey, token);
            return true;
        }

        public async Task<SiteConfigurationDto?> GetConfigurationAsync(string configKey, CancellationToken token = default)
        {
            return await _hybridCache.GetOrCreateAsync(configKey, async cancel =>
            {
                SiteConfiguration? siteConfiguration = await _siteConfigurationRepository.GetConfigurationAsync(configKey,true,token);
                if ( siteConfiguration is null || siteConfiguration.IsDeleted )
                {
                    return null;
                }
                return _mapper.Map<SiteConfigurationDto>(siteConfiguration);
            }, tags: [configKey, "site-configurations"], cancellationToken:token);
        }

        public async Task<SiteConfigurationDto?> GetConfigurationByIdAsync(int configId, CancellationToken token = default)
        {
            return await _hybridCache.GetOrCreateAsync($"site-configuration-id:{configId}", async cancel =>
            {
                SiteConfiguration? siteConfiguration = await _siteConfigurationRepository.GetEntityByIdAsync(configId,true,token);
                if ( siteConfiguration is null || siteConfiguration.IsDeleted )
                {
                    return null;
                }
                return _mapper.Map<SiteConfigurationDto>(siteConfiguration);
            }, tags: ["site-configurations", $"site-configuration-id:{configId}"], cancellationToken: token);
        }

        public async Task<PagingDto<SiteConfigurationDto>> PagingConfigurationsAsync(string? keyword = null, int pageIndex = 1, int pageSize = 20, CancellationToken token = default)
        {
            pageIndex = Math.Max(1, pageIndex);
            int skip = (pageIndex - 1) * pageSize;
            var specification = SiteConfigurationSpecification.CreatePaging(keyword,skip,pageSize,true,false);
            int totalCount = await _siteConfigurationRepository.CountAsync(specification, token);
            var configurations = await _siteConfigurationRepository.GetAllEntitiesAsync(specification,token);
            int pageCount = (int)Math.Ceiling((double)totalCount / (double)pageSize);
            pageCount = Math.Max(1, pageCount);
            var configurationDtos = _mapper.Map<List<SiteConfigurationDto>>(configurations)??[];
            return new PagingDto<SiteConfigurationDto>()
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                PageSize = pageSize,
                DataCount = totalCount,
                Items = configurationDtos
            };
        }

        public async Task<SiteConfigurationDto?> UpdateConfigurationAsync(int configId, UpdateConfigurationRequest request, CancellationToken token = default)
        {
            SiteConfiguration? siteConfiguration = await _siteConfigurationRepository.GetEntityByIdAsync(configId,false,token);
            if(siteConfiguration is null || siteConfiguration.IsDeleted )
            {
                return null;
            }
            SiteConfigurationDto dto = _mapper.Map<SiteConfigurationDto>(siteConfiguration);
            dto = _mapper.Map<SiteConfigurationDto>(request);
            siteConfiguration = _mapper.Map<SiteConfiguration>(dto);
            _siteConfigurationRepository.Update(siteConfiguration);
            await _siteConfigurationRepository.SaveChangesAsync(token);
            await _hybridCache.SetAsync(dto.ConfigKey, dto, tags: [dto.ConfigKey, "site-configurations"], cancellationToken: token);
            return _mapper.Map<SiteConfigurationDto>(siteConfiguration);
        }

        public async Task<SiteConfigurationDto?> UpdateConfigurationEnabledAsync(int configId, PatchConfigurationRequest request, CancellationToken token = default)
        {
            SiteConfiguration? siteConfiguration = await _siteConfigurationRepository.GetEntityByIdAsync(configId,false,token);
            if ( siteConfiguration is null || siteConfiguration.IsDeleted )
            {
                return null;
            }
            if(siteConfiguration.IsEnabled == request.IsEnabled )
            {
                return _mapper.Map<SiteConfigurationDto>(siteConfiguration);
            }
            siteConfiguration.IsEnabled = request.IsEnabled;
            _siteConfigurationRepository.Update(siteConfiguration);
            await _siteConfigurationRepository.SaveChangesAsync();
            SiteConfigurationDto dto = _mapper.Map<SiteConfigurationDto>(siteConfiguration);
            if ( !request.IsEnabled )
            {
                await _hybridCache.RemoveAsync(siteConfiguration.ConfigKey, token);
            }
            else
            {
                await _hybridCache.SetAsync(dto.ConfigKey, dto, tags: [dto.ConfigKey, "site-configurations"], cancellationToken: token);
            }
            return dto;
        }

        public async Task<SiteConfigurationDto?> UpdateConfigurationValueByKeyAsync(string configKey, string? configValue, CancellationToken token = default)
        {
            SiteConfiguration? siteConfiguration = await _siteConfigurationRepository.GetConfigurationAsync(configKey,false,token);
            if ( siteConfiguration is null || siteConfiguration.IsDeleted )
            {
                return null;
            }
            siteConfiguration.ConfigValue = configValue;
            _siteConfigurationRepository.Update(siteConfiguration);
            await _siteConfigurationRepository.SaveChangesAsync(token);
            SiteConfigurationDto dto = _mapper.Map<SiteConfigurationDto>(siteConfiguration);
            await _hybridCache.SetAsync(dto.ConfigKey, dto, tags: [dto.ConfigKey, "site-configurations"], cancellationToken: token);
            return dto;
        }
    }
}
