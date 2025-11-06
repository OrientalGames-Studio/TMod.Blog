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

        public static ISpecification<Comment> CreatePagingArticleComment(Guid articleId,int skip,int take)
        {
            CommentSpecification specification = new CommentSpecification(c=>c.ArticleId == articleId && (c.ParentId == null || c.ParentId == Guid.Empty));
            specification.ApplyPaging(skip, take);
            specification.ApplyOrderBy(c => c.CreateDate);
            return specification;
        }

        public static ISpecification<Comment> CreatePagingCommentReplies(Guid commentId,int skip,int take)
        {
            CommentSpecification specification = new CommentSpecification(c=>c.ParentId == commentId);
            specification.ApplyPaging(skip, take);
            specification.ApplyOrderBy(c => c.CreateDate);
            return specification;
        }
    }
}
