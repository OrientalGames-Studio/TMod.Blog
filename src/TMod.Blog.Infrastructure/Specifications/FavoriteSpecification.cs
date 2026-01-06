using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Specifications;

namespace TMod.Blog.Infrastructure.Specifications
{
    public sealed class FavoriteSpecification : BaseSpecification<Favorite>
    {
        public static ISpecification<Favorite> CreateCountFavoriteByTarget(Guid targetId,FavoriteTypeEnum favoriteTypeEnum,bool includeDelete = false)
        {
            FavoriteSpecification specification = new FavoriteSpecification();
            specification.AddCriteria(f=>f.TargetId == targetId && f.FavoriteType == favoriteTypeEnum);
            if ( !includeDelete )
            {
                specification.AddCriteria(f=>!f.IsDeleted);
            }
            specification.EnabledNoTracking();
            return specification;
        }

        public static ISpecification<Favorite> CreateGetIsIFavoriteIt(Guid targetId,FavoriteTypeEnum favoriteTypeEnum,string fingerprint,string clientIp,bool includeDelete = false)
        {
            FavoriteSpecification specification = new FavoriteSpecification();
            specification.AddCriteria(f=>f.TargetId == targetId && f.FavoriteType == favoriteTypeEnum && f.Fingerprint == fingerprint && f.ClientIp == clientIp);
            if ( !includeDelete )
            {
                specification.AddCriteria(f=>!f.IsDeleted);
            }
            specification.EnabledNoTracking();
            return specification;
        }

        public static ISpecification<Favorite> CreateGetFavoriteList(Guid targetId,FavoriteTypeEnum favoriteTypeEnum,bool includeDelete = false)
        {
            FavoriteSpecification specification = new FavoriteSpecification();
            specification.AddCriteria(f=>f.TargetId == targetId && f.FavoriteType == favoriteTypeEnum);
            if ( !includeDelete )
            {
                specification.AddCriteria(f=>!f.IsDeleted);
            }
            specification.EnabledNoTracking();
            return specification;
        }
    }
}
