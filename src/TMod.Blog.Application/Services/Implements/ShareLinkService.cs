using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Text;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces.Services;
using TMod.Blog.Domain.Interfaces.UnitOfWorks;
using TMod.Blog.Infrastructure.Specifications;

namespace TMod.Blog.Application.Services.Implements
{
    internal class ShareLinkService : IShareLinkService
    {
        private readonly ILogger<ShareLinkService> _logger;
        private readonly IArtifactUnitOfWork _unitOfWork;
        private readonly IShortCodeService _shortCodeService;
        private readonly ISiteConfigurationService _siteConfigurationService;

        public ShareLinkService(ILogger<ShareLinkService> logger, IArtifactUnitOfWork unitOfWork, IShortCodeService shortCodeService, ISiteConfigurationService siteConfigurationService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _shortCodeService = shortCodeService;
            _siteConfigurationService = siteConfigurationService;
        }

        public async Task<string> CreateShareAsync(ShareTargetTypeEnum targetType, Guid targetId, string clientIp, DateTime? expireAt = null, CancellationToken token = default)
        {
            bool isAutoExpire = expireAt is not null && expireAt.HasValue && expireAt.Value > DateTime.Today.AddSeconds(-1);
            string? siteIsEnableShareConfig = (await _siteConfigurationService.GetConfigurationAsync(SITE_IS_SHARE_ENABLE))?.ConfigValue;
            if(string.IsNullOrWhiteSpace(siteIsEnableShareConfig) || !bool.TryParse(siteIsEnableShareConfig, out bool isEnableShare) || !isEnableShare )
            {
                return "";
            }
            ISharable? sharable;
            if(targetType is ShareTargetTypeEnum.Article )
            {
                sharable = await _unitOfWork.ArticleRepository.GetEntityByIdAsync(targetId,true,token);
            }
            else
            {
                sharable = await _unitOfWork.CommentRepository.GetEntityByIdAsync(targetId,true,token);
            }
            isEnableShare = sharable?.IsShareEnabled == true;
            if ( !isEnableShare )
            {
                return "";
            }
            string shortCode = await _shortCodeService.GenerateShortCodeAsync(SHORT_CODE_DEFAULT_LENGTH);
            ShareLink shareLink = new ShareLink()
            {
                Id = 0,
                TargetType = targetType,
                TargetId = targetId,
                ShortCode = shortCode,
                CreatorIp = clientIp,
                AutoExpire = isAutoExpire,
                ExpireDate = expireAt
            };
            await _unitOfWork.ShareLinkRepository.AddAsync(shareLink, token);
            await _unitOfWork.SaveChangesAsync(token);
            return shortCode;
        }

        public async Task<bool> DeleteShareAsync(int id, CancellationToken token = default)
        {
            var sharelink = await _unitOfWork.ShareLinkRepository.GetEntityByIdAsync(id, true, token);
            if ( sharelink is null || sharelink.IsDeleted)
            {
                return false;
            }
            _unitOfWork.ShareLinkRepository.Delete(sharelink);
            await _unitOfWork.SaveChangesAsync(token);
            return true;
        }

        public async Task<ISharable?> LoadByShareAsync(string shortCode, CancellationToken token = default)
        {
            var specification = ShareLinkSpecification.CreateGetShareLinkByShortCode(shortCode);
            var sharelink = await _unitOfWork.ShareLinkRepository.GetEntityAsync(specification,token);
            if(sharelink is null )
            {
                return null;
            }
            if ( sharelink.IsDeleted )
            {
                return null;
            }
            if(sharelink.AutoExpire && DateTime.Now > sharelink.ExpireDate )
            {
                return null;
            }
            ISharable? target;
            if(sharelink.TargetType is ShareTargetTypeEnum.Article )
            {
                var articleSpecification = ArticleSpecification.CreateGetDetail(sharelink.TargetId,true);
                target = await _unitOfWork.ArticleRepository.GetEntityAsync(articleSpecification);
            }
            else
            {
                var commentSpecification = CommentSpecification.CreateGetCommentById(sharelink.TargetId);
                target = await _unitOfWork.CommentRepository.GetEntityAsync(commentSpecification);
            }
            return target;
        }
    }
}
