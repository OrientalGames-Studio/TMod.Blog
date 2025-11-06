using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Specifications
{
    /// <summary>
    /// 查询执行器
    /// </summary>
    public static class SpecificationEvaluator
    {
        /// <summary>
        /// 将 Specification 转换成 LINQ 表达式
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="input">源数据</param>
        /// <param name="specification">筛选条件</param>
        /// <returns></returns>
        public static IQueryable<TEntity> GetQuery<TEntity>(IQueryable<TEntity> input
            ,ISpecification<TEntity> specification) where TEntity : class
        {
            var query = input;

            if(specification.Criteria is not null && specification.Criteria.Count > 0)
            {
                query = specification.Criteria.Aggregate(query, (current, criteria) => current.Where(criteria));
            }

            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            query = specification.IncludeStrings.Aggregate(query,(current,include)=>current.Include(include));

            IOrderedQueryable<TEntity>? orderedQuery = null;
            if(specification.OrderBy is not null )
            {
                orderedQuery = query.OrderBy(specification.OrderBy);
            } else if(specification.OrderByDescending is not null )
            {
                orderedQuery = query.OrderByDescending(specification.OrderByDescending);
            }

            if(orderedQuery is not null && specification.ThenByChain is not null && specification.ThenByChain.Count > 0)
            {
                foreach ( var thenBy in specification.ThenByChain )
                {
                    orderedQuery = orderedQuery.ThenBy(thenBy);
                }
                query = orderedQuery;
            }

            if ( specification.Skip.HasValue )
            {
                query = query.Skip(specification.Skip.Value);
            }

            if ( specification.Take.HasValue )
            {
                query = query.Take(specification.Take.Value);
            }

            if ( specification.IsNoTracking )
            {
                query = query.AsNoTracking();
            }

            return query;
        }
    }
}
