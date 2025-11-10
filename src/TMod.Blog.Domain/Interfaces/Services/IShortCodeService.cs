using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Interfaces.Services
{
    /// <summary>
    /// 短码服务：生成 Base62(short) 的短码，内部对 Snowflake ID 做 keyed-XOR 混淆以防止预测。
    /// </summary>
    public interface IShortCodeService
    {
        /// <summary>
        /// 生成短码
        /// </summary>
        /// <param name="length">短码长度</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GenerateShortCodeAsync(int length = 8,CancellationToken cancellationToken = default);

        /// <summary>
        /// 解析短码回溯出 snowflake 信息（如果解码失败返回 null）
        /// </summary>
        /// <param name="shortCode">短码</param>
        /// <returns></returns>
        (DateTime createDate,int workerId,long sequence)? DecodeShortCode(string shortCode);
    }
}
