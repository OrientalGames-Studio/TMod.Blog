using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Constants
{
    /// <summary>
    /// 配置项 Key 静态定义类
    /// </summary>
    public static class ConfigKeyConstants
    {
        /// <summary>
        /// 站点标题配置项的 Key
        /// </summary>
        public const string SITE_TITLE = nameof(SITE_TITLE);

        /// <summary>
        /// 短码服务的 Worker Id 配置项的 Key
        /// </summary>
        public const string SITE_SHORT_CODE_WORKER_ID = nameof(SITE_SHORT_CODE_WORKER_ID);

        /// <summary>
        /// 短码服务的 epoch 配置项的 Key
        /// </summary>
        public const string SITE_SHORT_CODE_EPOCH = nameof(SITE_SHORT_CODE_EPOCH);

        /// <summary>
        /// 短码服务的密钥配置项的 Key
        /// </summary>
        public const string SITE_SHORT_CODE_SECRET_KEY = nameof(SITE_SHORT_CODE_SECRET_KEY);

        /// <summary>
        /// 短码服务生成的短码最少几位配置项的 Key
        /// </summary>
        public const string SITE_SHORT_CODE_MIN_LENGTH = nameof(SITE_SHORT_CODE_MIN_LENGTH);

        /// <summary>
        /// 短码服务生成的短码最多几位配置项的 Key
        /// </summary>
        public const string SITE_SHORT_CODE_MAX_LENGTH = nameof(SITE_SHORT_CODE_MAX_LENGTH);

        /// <summary>
        /// 是否允许分享配置项的 Key
        /// </summary>
        public const string SITE_IS_SHARE_ENABLE = nameof(SITE_IS_SHARE_ENABLE);
    }
}
