using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Specifications;

namespace TMod.Blog.Domain.Interfaces
{
    /// <summary>
    /// 通用只读仓储接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public interface IReadOnlyRepository<TEntity,TKey> where TEntity : class
    {
        /// <summary>
        /// 根据主键查询实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="asNoTracking">是否跟踪实体数据变化</param>
        /// <returns></returns>
        Task<TEntity?> GetEntityByIdAsync(TKey id,bool asNoTracking = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// 查询全部实体
        /// </summary>
        /// <param name="asNoTracking">是否跟踪实体数据变化</param>
        /// <returns></returns>
        Task<IReadOnlyList<TEntity>> GetAllEntitiesAsync(bool asNoTracking = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// <inheritdoc cref="GetAllEntitiesAsync(bool)"/>
        /// </summary>
        /// <param name="specification">筛选条件</param>
        /// <returns></returns>
        Task<IReadOnlyList<TEntity>> GetAllEntitiesAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据筛选条件查询实体
        /// </summary>
        /// <param name="specification">筛选条件</param>
        /// <returns></returns>
        Task<TEntity?> GetEntityAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

        /// <summary>
        /// 判断筛选条件是否成立
        /// </summary>
        /// <param name="specification">筛选条件</param>
        /// <returns></returns>
        Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据筛选条件计算符合条件的实体数量
        /// </summary>
        /// <param name="specification">筛选条件</param>
        /// <returns></returns>
        Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    }
}
