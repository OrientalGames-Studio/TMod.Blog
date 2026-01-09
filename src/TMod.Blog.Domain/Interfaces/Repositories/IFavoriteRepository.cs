using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Domain.Interfaces.Repositories
{
    public interface IFavoriteRepository : IRepository<Favorite, Guid>, IReadOnlyRepository<Favorite, Guid>
    {
        Task<int> CountFavoriteByIdAsync(Guid targetId, FavoriteTypeEnum favoriteType, CancellationToken token = default);

        Task<bool> GetTargetIsFavoirtedAsync(Guid targetId,FavoriteTypeEnum favoriteTypeEnum,string fingerprint,string clientIp, CancellationToken token = default);

        Task<IReadOnlyList<Favorite>> GetFavoriteListAsync(Guid targetId, FavoriteTypeEnum favoriteType, CancellationToken token = default);
    }
}
