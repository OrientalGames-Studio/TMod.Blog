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
    internal class ShareLinkRepository(TMod_Blog_RW_Context context) : BlogRepository<ShareLink, int>(context), IShareLinkRepository
    {
        public async Task<ShareLink?> GetShareByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
        {
            ISpecification<ShareLink> specification = ShareLinkSpecification.CreateGetShareLinkByShortCode(shortCode);
            return await GetEntityAsync(specification, cancellationToken);
        }

        public async Task<bool> GetShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken = default)
        {
            ISpecification<ShareLink> specification = ShareLinkSpecification.CreateGetShareLinkByShortCode(shortCode);
            ShareLink? shareLink = await GetEntityAsync(specification, cancellationToken);
            if(shareLink is not null )
            {
                return true;
            }
            return false;
        }
    }
}
