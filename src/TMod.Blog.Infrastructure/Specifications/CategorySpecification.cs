using Microsoft.EntityFrameworkCore;

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
            CategorySpecification specification = new CategorySpecification();
            specification.AddInclude(c => c.Parent);
            specification.ApplyPaging(skip, take);
            if(parentId is not null && parentId.Value != Guid.Empty )
            {
                specification.AddCriteria(c => c.ParentId == parentId);
            }
            if ( !showDeleted )
            {
                specification.AddCriteria(c => !c.IsDeleted);
            }
            specification.EnabledNoTracking();
            return specification;
        }

        public static ISpecification<Category> CreatePagingCategoriesByParentIdWithNameFilter(Guid? parentId,string? categoryName, int skip, int take, bool showDeleted = false)
        {
            CategorySpecification specification = (CategorySpecification)CreatePagingCategoriesByParentId(parentId,skip,take,showDeleted);
            if ( !string.IsNullOrWhiteSpace(categoryName) )
            {
                specification.AddCriteria(c => EF.Functions.Like(c.Name,$"%{categoryName}%"));
            }
            return specification;
        }

        public static ISpecification<Category> CreateGetCategoryByIdWithDetail(Guid categoryId,bool asNoTracking = true,bool showDeleted = false)
        {
            CategorySpecification specification = new CategorySpecification();
            specification.AddCriteria(c => c.Id == categoryId);
            specification.AddInclude(c => c.Parent);
            if ( asNoTracking )
            {
                specification.EnabledNoTracking();
            }
            if ( !showDeleted )
            {
                specification.AddCriteria(c => !c.IsDeleted);
            }
            return specification;
        }
    }
}
