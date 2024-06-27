using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models;

namespace TMod.Blog.Data.Repositories.Implements
{
    internal class ConfigurationRepository : BaseIntKeyRepository<Configuration>, IConfigurationRepository
    {
        private readonly ILogger<ConfigurationRepository> _logger;
        public ConfigurationRepository(BlogContext blogContext, ILoggerFactory loggerFactory) : base(blogContext, loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ConfigurationRepository>();
        }

        public async Task BatchRemoveConfigurationByIdAsync(params int[] configurationIds)
        {
            using ( var trans = await base.BlogContext.Database.BeginTransactionAsync() )
            {
                try
                {
                    foreach ( int id in configurationIds )
                    {
                        Configuration? configuration = await base.LoadAsync(id);
                        if(configuration is null )
                        {
                            _logger.LogWarning($"无法获取到Id是{id}的配置项");
                            continue;
                        }
                        await base.RemoveAsync(configuration);
                    }
                    await trans.CommitAsync();
                    await base.BlogContext.SaveChangesAsync();
                }
                catch ( Exception ex )
                {
                    trans.Rollback();
                    _logger.LogError(ex, $"根据 Id 批量删除配置项时发生异常，Id:({string.Join(",", configurationIds ?? [])})");
                    throw;
                }
            }
        }
    }
}
