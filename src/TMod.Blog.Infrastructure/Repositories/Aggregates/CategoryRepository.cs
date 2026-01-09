using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Domain.Specifications;
using TMod.Blog.Infrastructure.Contextes;
using TMod.Blog.Infrastructure.Specifications;

namespace TMod.Blog.Infrastructure.Repositories.Aggregates
{
    internal class CategoryRepository(TMod_Blog_RW_Context context) : BlogRepository<Category, Guid>(context), ICategoryRepository
    {
        public async Task<IReadOnlyList<Category>> PagingCategoriesByParentIdAsync(Guid? parentId, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            int skip = (Math.Max(1,pageIndex) - 1) * pageSize;
            ISpecification<Category> specification = CategorySpecification.CreatePagingCategoriesByParentId(parentId,skip,pageSize);
            return await GetAllEntitiesAsync(specification,cancellationToken);
        }
    }
}
