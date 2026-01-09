using Microsoft.Extensions.Logging;

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
    internal class FavoriteRepository : BlogRepository<Favorite, Guid>, IFavoriteRepository
    {
        private readonly ILogger<FavoriteRepository> _logger;
        public FavoriteRepository(TMod_Blog_RW_Context context,ILogger<FavoriteRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public Task<int> CountFavoriteByIdAsync(Guid targetId, FavoriteTypeEnum favoriteType, CancellationToken token = default)
        {
            ISpecification<Favorite> specification = FavoriteSpecification.CreateCountFavoriteByTarget(targetId, favoriteType);
            return CountAsync(specification, token);
        }

        public Task<IReadOnlyList<Favorite>> GetFavoriteListAsync(Guid targetId, FavoriteTypeEnum favoriteType, CancellationToken token = default)
        {
            ISpecification<Favorite> specification = FavoriteSpecification.CreateGetFavoriteList(targetId,favoriteType);
            return GetAllEntitiesAsync(specification, token);
        }

        public Task<bool> GetTargetIsFavoirtedAsync(Guid targetId, FavoriteTypeEnum favoriteTypeEnum, string fingerprint, string clientIp, CancellationToken token = default)
        {
            ISpecification<Favorite> specification = FavoriteSpecification.CreateGetIsIFavoriteIt(targetId,favoriteTypeEnum,fingerprint, clientIp);
            return AnyAsync(specification, token);
        }
    }
}
