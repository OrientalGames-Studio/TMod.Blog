using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Interfaces;
using TMod.Blog.Infrastructure.Contextes;

namespace TMod.Blog.Infrastructure.Repositories
{
    internal class BlogRepository<TEntity, TKey> : ReadOnlyBlogRepository<TEntity, TKey>, IRepository<TEntity, TKey> where TEntity : class
    {
        public BlogRepository(TMod_Blog_RW_Context context) : base(context)
        {
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await set.AddAsync(entity, cancellationToken);
        }

        public async Task BatchAddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await set.AddRangeAsync(entities, cancellationToken);
        }

        public bool BatchDelete(IEnumerable<TEntity> entities, bool hardDelete = false, bool failureOnFirstFail = false)
        {
            foreach ( var entity in entities )
            {
                var ok = Delete(entity,hardDelete);
                if(failureOnFirstFail && !ok )
                {
                    return false;
                }
            }
            return true;
        }

        public bool Delete(TEntity entity, bool hardDelete = false)
        {
            if ( hardDelete )
            {
                set.Remove(entity);
            }
            else
            {
                var prop = entity.GetType().GetProperty("IsDeleted");
                if ( prop != null )
                    prop.SetValue(entity, true);
                else
                    set.Remove(entity);
            }

            return true;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public bool Update(TEntity entity)
        {
            try
            {
                set.Update(entity);
                return true;
            }
            catch ( Exception )
            {
                return false;
            }
        }
    }
}
