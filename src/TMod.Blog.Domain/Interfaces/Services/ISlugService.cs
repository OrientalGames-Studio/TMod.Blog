using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Interfaces.Services
{
    /// <summary>
    /// Slug服务
    /// </summary>
    public interface ISlugService
    {
        /// <summary>
        /// 生成 SEO 友好的 Slug
        /// </summary>
        /// <param name="title">字符串</param>
        /// <param name="maxLength">Slug 最大长度</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GenerateSlugAsync(string title,int? maxLength = null,CancellationToken cancellationToken = default);
    }
}
