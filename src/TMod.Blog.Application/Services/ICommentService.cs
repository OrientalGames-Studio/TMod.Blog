using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Comment;

namespace TMod.Blog.Application.Services
{
    public interface ICommentService
    {
        Task<CommentDto> CreateCommentAsync(CreateCommentRequest request,string authorIp, CancellationToken cancellationToken = default);

        Task<bool> DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken = default);

        Task<PagingDto<CommentDto>> PagingCommentsByArticleAsync(Guid articleId,int pageIndex = 1,int pageSize = 20, CancellationToken cancellationToken = default);

        Task<PagingDto<CommentDto>> PagingCommentsByCommentAsync(Guid commentId,int pageIndex = 1,int pageSize = 20, CancellationToken cancellationToken = default);

        Task<CommentDto?> GetCommentByIdAsync(Guid commentId, CancellationToken cancellationToken = default);

        Task FavoriteAsync(Guid commentId,CancellationToken cancellationToken = default);
    }
}
