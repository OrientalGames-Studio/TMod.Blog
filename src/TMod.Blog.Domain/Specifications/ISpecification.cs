using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Specifications
{
    /// <summary>
    /// 查询条件的抽象规范接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface ISpecification<TEntity>
    {
        /// <summary>
        /// 查询条件表达式
        /// </summary>
        List<Expression<Func<TEntity, bool>>> Criteria { get; }

        /// <summary>
        /// Include 的导航属性
        /// </summary>
        List<Expression<Func<TEntity,object>>> Includes { get; }

        /// <summary>
        /// 字符串形式的 Include 导航属性
        /// </summary>
        [Obsolete("为了更好的支持 AOT，建议使用 {Includes}")]
        List<string> IncludeStrings { get; }

        /// <summary>
        /// 排序（升序）
        /// </summary>
        Expression<Func<TEntity,object>>? OrderBy { get; }

        /// <summary>
        /// 排序（降序）
        /// </summary>
        Expression<Func<TEntity,object>>? OrderByDescending { get; }

        /// <summary>
        /// 多级 OrderBy
        /// </summary>
        List<Expression<Func<TEntity, object>>> ThenByChain { get; }

        /// <summary>
        /// 偏移量（分页）
        /// </summary>
        int? Skip { get; }

        /// <summary>
        /// 获取数据量（分页）
        /// </summary>
        int? Take { get; }

        /// <summary>
        /// 是否启用 AsNoTracking
        /// </summary>
        bool IsNoTracking { get; }
    }
}
