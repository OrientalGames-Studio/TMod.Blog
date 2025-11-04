using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Specifications;

namespace TMod.Blog.Infrastructure.Specifications
{
    public sealed class CategorySpecification : BaseSpecification<Category>
    {
        public CategorySpecification()
        {
        }

        public CategorySpecification(Expression<Func<Category, bool>> criteria) : base(criteria)
        {
        }

        public static ISpecification<Category> CreatePagingCategoriesByParentId(Guid? parentId, int skip, int take, bool showDeleted = false)
        {
            CategorySpecification specification = new CategorySpecification(c => c.ParentId == parentId);
            specification.AddInclude(c => c.Parent);
            specification.ApplyPaging(skip, take);
            if ( !showDeleted )
            {
                specification.AddCriteria(c => !c.IsDeleted);
            }
            return specification;
        }
    }
}
