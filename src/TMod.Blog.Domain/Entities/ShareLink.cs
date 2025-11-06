using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Entities
{
    public sealed class ShareLink:BaseEntity<int>
    {
        /// <summary>
        /// 分享目标类型
        /// </summary>
        public ShareTargetTypeEnum TargetType {  get; set; }

        /// <summary>
        /// 分享目标 ID
        /// </summary>
        /// <remarks>因为文章和评论明确都是 Guid 主键，所以这里就写死了，不考虑其他类型</remarks>
        public Guid TargetId { get; set; }

        /// <summary>
        /// 分享短码
        /// </summary>
        [MaxLength(12)]
        public string? ShortCode { get; set; }

        /// <summary>
        /// 分享者 IP
        /// </summary>
        [MaxLength(64)]
        public string? CreatorIp { get; set; }

        [NotMapped]
        public ISharable? Target {  get; set; }
    }
}
