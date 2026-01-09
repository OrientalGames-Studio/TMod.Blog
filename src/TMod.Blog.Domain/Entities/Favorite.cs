using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Entities
{
    public class Favorite:BaseEntity<Guid>
    {
        public Guid TargetId { get; set; }

        /// <summary>
        /// 客户端指纹
        /// </summary>
        public required string Fingerprint { get; set; }

        public required string ClientIp { get; set; }

        public FavoriteTypeEnum FavoriteType { get; set; }
    }
}
