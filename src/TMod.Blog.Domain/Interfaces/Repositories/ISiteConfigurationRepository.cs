using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Domain.Interfaces.Repositories
{
    /// <summary>
    /// 站点配置聚合根仓储接口
    /// </summary>
    public interface ISiteConfigurationRepository:IRepository<SiteConfiguration,int>,IReadOnlyRepository<SiteConfiguration,int>
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="configKey">配置项</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SiteConfiguration?> GetConfigurationAsync(string configKey,CancellationToken cancellationToken = default);

        /// <summary>
        /// 分页查询配置项
        /// </summary>
        /// <param name="keyword">配置项模糊筛选</param>
        /// <param name="showDisabled">是否展示被禁用的配置项</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">单页数据量</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyList<SiteConfiguration>> PagingConfigurationsAsync(string? keyword = null,bool showDisabled = false, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    }
}
