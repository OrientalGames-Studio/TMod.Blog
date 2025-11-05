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
    public sealed class ArticleSpecification : BaseSpecification<Article>
    {
        public ArticleSpecification()
        {
        }

        public ArticleSpecification(Expression<Func<Article, bool>> criteria) : base(criteria)
        {
        }

        public static ISpecification<Article> CreateGetDetail(Guid articleId,bool showDeleted = false)
        {
            ArticleSpecification specification = new ArticleSpecification(a=>a.Id == articleId);
            specification.AddInclude(c => c.Category);
            specification.AddInclude(c => c.Tags);
            specification.AddInclude(c => c.Comments);
            if ( !showDeleted )
            {
                specification.AddCriteria(c => !c.IsDeleted);
            }
            return specification;
        }

        public static ISpecification<Article> CreatePagingByCategoryId(Guid categoryId, int skip, int take, string? keyword = null,ArticleStatusEnum articleStatus = ArticleStatusEnum.Draft | ArticleStatusEnum.Published | ArticleStatusEnum.Unpublished, bool showDeleted = false)
        {
            ArticleSpecification specification = new ArticleSpecification(a=>a.CategoryId == categoryId);
            specification.ApplyPaging(skip, take);
            specification.AddInclude(c => c.Category);
            specification.AddInclude(c => c.Tags);
            specification.AddCriteria(a => ( a.Status & articleStatus ) == articleStatus);
            if ( !string.IsNullOrWhiteSpace(keyword) )
            {
                specification.AddCriteria(a=>a.Title.Contains(keyword,StringComparison.InvariantCultureIgnoreCase) || a.Slug.Contains(keyword,StringComparison.InvariantCultureIgnoreCase));
            }
            if ( !showDeleted )
            {
                specification.AddCriteria(c => !c.IsDeleted);
            }
            return specification;
        }

        public static ISpecification<Article> CreatePaging(int skip, int take, string? keyword = null, ArticleStatusEnum articleStatus = ArticleStatusEnum.Draft | ArticleStatusEnum.Published | ArticleStatusEnum.Unpublished, bool showDeleted = false)
        {
            ArticleSpecification specification = new ArticleSpecification();
            specification.ApplyPaging(skip, take);
            specification.AddInclude(c => c.Category);
            specification.AddInclude(c => c.Tags);
            specification.AddCriteria(a => ( a.Status & articleStatus ) == articleStatus);
            if ( !string.IsNullOrWhiteSpace(keyword) )
            {
                specification.AddCriteria(a => a.Title.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) || a.Slug.Contains(keyword, StringComparison.InvariantCultureIgnoreCase));
            }
            if ( !showDeleted )
            {
                specification.AddCriteria(c => !c.IsDeleted);
            }
            return specification;
        }
    }
}
