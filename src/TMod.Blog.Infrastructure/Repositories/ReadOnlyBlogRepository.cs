using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Interfaces;
using TMod.Blog.Domain.Specifications;
using TMod.Blog.Infrastructure.Contextes;

namespace TMod.Blog.Infrastructure.Repositories
{
    internal class ReadOnlyBlogRepository<TEntity, TKey>(TMod_Blog_RW_Context context) : IReadOnlyRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly TMod_Blog_RW_Context context = context;
        protected readonly DbSet<TEntity> set = context.Set<TEntity>();

        public async Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            var query = SpecificationEvaluator.GetQuery(set.AsQueryable(),specification);
            return await query.AnyAsync(cancellationToken);
        }

        public async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            var query = SpecificationEvaluator.GetQuery(set.AsQueryable(),specification);
            return await query.CountAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllEntitiesAsync(bool asNoTracking = false, CancellationToken cancellationToken = default)
        {
            var query = set.AsQueryable();
            if ( asNoTracking )
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllEntitiesAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            var query = SpecificationEvaluator.GetQuery(set.AsQueryable(),specification);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetEntityAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            var query = SpecificationEvaluator.GetQuery(set.AsQueryable(),specification);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TEntity?> GetEntityByIdAsync(TKey id, bool asNoTracking = false, CancellationToken cancellationToken = default)
        {
            if ( asNoTracking )
            {
                var keyProperty = context.Model.FindEntityType(typeof(TEntity))
                    ?.FindPrimaryKey()?.Properties?.FirstOrDefault();
                if(keyProperty is null )
                {
                    return null;
                }
                var parameter = Expression.Parameter(typeof(TEntity),"e");
                var lambda = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.Property(parameter, keyProperty.Name),
                    Expression.Constant(id)
                    ),
            parameter
                );
                return await set.AsNoTracking().FirstOrDefaultAsync(lambda, cancellationToken);
            }
            return await set.FindAsync([id], cancellationToken);
        }
    }
}
