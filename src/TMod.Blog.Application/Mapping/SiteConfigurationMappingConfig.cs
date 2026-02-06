using Mapster;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.SiteConfiguration;
using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Application.Mapping
{
    internal class SiteConfigurationMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<SiteConfiguration, SiteConfigurationDto>();
            config.NewConfig<SiteConfigurationDto, SiteConfiguration>();

            // Requests should map to DTOs only
            config.NewConfig<CreateConfigurationRequest, SiteConfigurationDto>();
            config.NewConfig<UpdateConfigurationRequest, SiteConfigurationDto>();
        }
    }
}
