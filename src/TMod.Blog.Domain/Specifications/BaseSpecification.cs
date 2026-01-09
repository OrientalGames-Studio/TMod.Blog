using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Specifications
{
    public abstract class BaseSpecification<TEntity> : ISpecification<TEntity> where TEntity : class
    {
        public List<Expression<Func<TEntity, bool>>> Criteria { get; protected set; } = [];

        public List<Expression<Func<TEntity, object>>> Includes { get; protected set; } = [];

        public List<string> IncludeStrings { get; protected set; } = [];

        public Expression<Func<TEntity, object>>? OrderBy { get; protected set; }

        public Expression<Func<TEntity, object>>? OrderByDescending { get; protected set; }

        public List<Expression<Func<TEntity, object>>> ThenByChain { get; protected set; } = [];

        public int? Skip { get; protected set; }

        public int? Take { get; protected set; }

        public bool IsNoTracking { get; protected set; } = false;

        public BaseSpecification() { }

        public BaseSpecification(Expression<Func<TEntity,bool>> criteria)
        {
            Criteria??=[];
            Includes ??= [];
            IncludeStrings ??= [];
            Criteria.Add(criteria);
        }

        protected void AddCriteria(Expression<Func<TEntity,bool>> criteriaExpression)
        {
            Criteria.Add(criteriaExpression);
        }

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddInclude(string includeExpressionString)
        {
            IncludeStrings.Add(includeExpressionString);
        }

        protected void ApplyOrderBy(Expression<Func<TEntity, object>>? orderByExpression) => OrderBy = orderByExpression;

        protected void ApplyThenBy(Expression<Func<TEntity,object>> thenByExpression)
        {
            if(OrderBy is null || OrderByDescending is null )
            {
                throw new NotSupportedException($"添加 {nameof(ThenByChain)} 之前必须先应用 {nameof(OrderBy)} 或 {nameof(OrderByDescending)}");
            }
            ThenByChain.Add(thenByExpression);
        }

        protected void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByExpression) => OrderByDescending = orderByExpression;

        protected void ApplyPaging(int? skip, int? take)
        {
            Skip = skip;
            Take = take;
        }

        protected void EnabledNoTracking() => IsNoTracking = !IsNoTracking;
    }
}
