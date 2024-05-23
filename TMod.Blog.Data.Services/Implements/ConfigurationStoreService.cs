using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Repositories;

namespace TMod.Blog.Data.Services.Implements
{
    internal class ConfigurationStoreService : IConfigurationStoreService
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILogger<ConfigurationStoreService> _logger;

        public ConfigurationStoreService(IConfigurationRepository configurationRepository, ILogger<ConfigurationStoreService> logger)
        {
            _configurationRepository = configurationRepository;
            _logger = logger;
        }
    }
}
