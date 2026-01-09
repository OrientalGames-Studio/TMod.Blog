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
    public sealed class TagSpecification:BaseSpecification<Tag>
    {
        public TagSpecification()
        {
        }

        public TagSpecification(Expression<Func<Tag, bool>> criteria) : base(criteria)
        {
        }

        public static ISpecification<Tag> CreateGetByName(string name,bool showDeleted = false)
        {
            TagSpecification specification = new TagSpecification(t=>StringComparer.InvariantCultureIgnoreCase.Compare(t.Name,name) == 0);
            if ( !showDeleted )
            {
                specification.AddCriteria(t => !t.IsDeleted);
            }
            return specification;
        }

        public static ISpecification<Tag> CreatePaging(string? keyword, int skip, int take, bool showDeleted = false)
        {
            TagSpecification specification = new TagSpecification();
            if ( !string.IsNullOrWhiteSpace(keyword) )
            {
                specification.AddCriteria(t => StringComparer.InvariantCultureIgnoreCase.Compare(t.Name, keyword) == 0);
            }
            if ( !showDeleted )
            {
                specification.AddCriteria(t => !t.IsDeleted);
            }
            specification.ApplyPaging(skip, take);
            specification.ApplyOrderBy(t => t.Name);
            return specification;
        }

        public static ISpecification<Tag> CreateGetAllByArticle(Guid articleId,bool showDeleted = false)
        {
            TagSpecification specification = new TagSpecification(t=>t.Articles.Any(a=> a.Id == articleId));
            specification.AddInclude(t => t.Articles);
            if ( !showDeleted )
            {
                specification.AddCriteria(t => !t.IsDeleted);
            }
            specification.EnabledNoTracking();
            return specification;
        }
    }
}
