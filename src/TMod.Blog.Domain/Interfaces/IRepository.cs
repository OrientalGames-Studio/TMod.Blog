using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Interfaces
{
    /// <summary>
    /// 通用仓储接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public interface IRepository<TEntity,TKey>:IReadOnlyRepository<TEntity,TKey> where TEntity : class
    {
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entities">实体</param>
        /// <returns></returns>
        Task BatchAddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        bool Update(TEntity entity);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="hardDelete">是否使用硬删除</param>
        /// <returns></returns>
        bool Delete(TEntity entity,bool hardDelete = false);

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="entities">实体</param>
        /// <param name="hardDelete">是否使用硬删除</param>
        /// <param name="failureOnFirstFail">是否在发生第一个失败时结束所有任务</param>
        /// <returns></returns>
        bool BatchDelete(IEnumerable<TEntity> entities,bool hardDelete = false,bool failureOnFirstFail = false);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
