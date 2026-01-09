using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Domain.Interfaces.Repositories
{
    /// <summary>
    /// 分享链聚合根仓储接口
    /// </summary>
    public interface IShareLinkRepository:IRepository<ShareLink,int>,IReadOnlyRepository<ShareLink,int>
    {
        /// <summary>
        /// 根据分享短码获取分享内容
        /// </summary>
        /// <param name="shortCode">分享短码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ShareLink?> GetShareByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// 判断分享短码是否存在
        /// </summary>
        /// <param name="shortCode">分享短码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> GetShortCodeExistsAsync(string shortCode,CancellationToken cancellationToken = default);
    }
}
