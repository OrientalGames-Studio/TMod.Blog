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
    public sealed class CommentSpecification : BaseSpecification<Comment>
    {
        public CommentSpecification()
        {
        }

        public CommentSpecification(Expression<Func<Comment, bool>> criteria) : base(criteria)
        {
        }

        public static ISpecification<Comment> CreatePagingArticleComment(Guid articleId,int skip,int take,SortRuleEnum sortRule)
        {
            CommentSpecification specification = new CommentSpecification(c=>c.ArticleId == articleId && (c.ParentId == null || c.ParentId == Guid.Empty));
            specification.ApplyPaging(skip, take);
            if(sortRule == SortRuleEnum.Default )
            {
                specification.ApplyOrderBy(c => c.CreateDate);
            }
            else
            {
                specification.ApplyOrderByDescending(c => c.CreateDate);
            }
            specification.EnabledNoTracking();
            return specification;
        }

        public static ISpecification<Comment> CreateCountPagingByArticle(Guid articleId)
        {
            CommentSpecification specification = new CommentSpecification(c=>c.ArticleId == articleId && (c.ParentId == null || c.ParentId == Guid.Empty));
            specification.EnabledNoTracking();
            return specification;
        }

        public static ISpecification<Comment> CreatePagingCommentReplies(Guid commentId,int skip,int take,SortRuleEnum sortRule)
        {
            CommentSpecification specification = new CommentSpecification(c=>c.ParentId == commentId);
            specification.ApplyPaging(skip, take);
            if ( sortRule == SortRuleEnum.Default )
            {
                specification.ApplyOrderBy(c => c.CreateDate);
            }
            else
            {
                specification.ApplyOrderByDescending(c => c.CreateDate);
            }
            specification.EnabledNoTracking();
            return specification;
        }

        public static ISpecification<Comment> CreateCountPagingByComment(Guid commentId)
        {
            CommentSpecification specification = new CommentSpecification(c=>c.ParentId == commentId);
            specification.EnabledNoTracking();
            return specification;
        }

        public static ISpecification<Comment> CreateGetCommentById(Guid commentId)
        {
            CommentSpecification specification = new CommentSpecification(c=>c.Id == commentId);
            specification.EnabledNoTracking();
            return specification;
        }
    }
}
