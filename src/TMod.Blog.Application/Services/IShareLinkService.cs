using System;
using System.Collections.Generic;
using System.Text;

using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Application.Services
{
    public interface IShareLinkService
    {
        Task<string> CreateShareAsync(ShareTargetTypeEnum targetType, Guid targetId, string clientIp, DateTime? expireAt = null, CancellationToken token = default);

        Task<bool> DeleteShareAsync(int id, CancellationToken token = default);

        Task<ISharable?> LoadByShareAsync(string shortCode, CancellationToken token = default); 
    }
}
