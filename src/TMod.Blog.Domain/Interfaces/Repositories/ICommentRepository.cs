using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Domain.Interfaces.Repositories
{
    /// <summary>
    /// 评论聚合根仓储接口
    /// </summary>
    public interface ICommentRepository:IRepository<Comment,Guid>,IReadOnlyRepository<Comment,Guid>
    {
        /// <summary>
        /// 按照文章分页查询评论
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">单页数据量</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Comment>> PagingArticleCommentAsync(Guid articleId, int pageIndex = 1, int pageSize = 20,CancellationToken cancellationToken = default);

        /// <summary>
        /// 按照评论分页查询子评论
        /// </summary>
        /// <param name="commentId">评论Id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">单页数据量</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Comment>> PagingCommentRepliesAsync(Guid commentId, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commentId">评论Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> DeleteCommentAsync(Guid commentId,CancellationToken cancellationToken = default);
    }
}
