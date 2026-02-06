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
    internal class CommentRepository(TMod_Blog_RW_Context context) : BlogRepository<Comment, Guid>(context), ICommentRepository
    {
        public async Task<bool> DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken = default)
        {
            Comment? meta = await GetEntityByIdAsync(commentId,cancellationToken:cancellationToken);
            if(meta is null )
            {
                return false;
            }
            meta.IsDeleted = true;
            meta.DeleteDate = DateTime.Now;
            Delete(meta);
            await SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IReadOnlyList<Comment>> PagingArticleCommentAsync(Guid articleId, int pageIndex = 1, int pageSize = 20, SortRuleEnum sortRule = SortRuleEnum.Latest, CancellationToken cancellationToken = default)
        {
            int skip = (Math.Max(1,pageIndex)-1)*pageSize;
            ISpecification<Comment> specification = CommentSpecification.CreatePagingArticleComment(articleId,skip,pageSize,sortRule);
            return await GetAllEntitiesAsync(specification,cancellationToken);
        }

        public async Task<IReadOnlyList<Comment>> PagingCommentRepliesAsync(Guid commentId, int pageIndex = 1, int pageSize = 20, SortRuleEnum sortRule = SortRuleEnum.Latest, CancellationToken cancellationToken = default)
        {
            int skip = (Math.Max(1,pageIndex)-1)*pageSize;
            ISpecification<Comment> specification = CommentSpecification.CreatePagingCommentReplies(commentId,skip,pageSize,sortRule);
            return await GetAllEntitiesAsync(specification,cancellationToken);
        }
    }
}
