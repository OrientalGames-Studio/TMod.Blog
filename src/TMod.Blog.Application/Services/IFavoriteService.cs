using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application.Services
{
    public interface IFavoriteService
    {
        Task FavoriteArticleAsync(Guid articleId,CancellationToken token = default);

        Task FavoriteCommentAsync(Guid commentId,CancellationToken token = default);

        Task<int> CountArticleFavoritesAsync(Guid articleId,CancellationToken token = default);

        Task<int> CountCommentFavoritesAsync(Guid commentId,CancellationToken token = default);

        Task<bool> GetArticleIsFavoritedAsync(Guid articleId,string fingerprint,string clientIp,CancellationToken token = default);

        Task<bool> GetCommentIsFavoritedAsync(Guid commentId, string fingerprint, string clientIp, CancellationToken token = default);

        Task<IReadOnlyList<string>> GetArticleFavoritedListAsync(Guid articleId, CancellationToken token = default);

        Task<IReadOnlyList<string>> GetCommentFavoirtedListAsync(Guid commentId, CancellationToken token = default);
    }
}
