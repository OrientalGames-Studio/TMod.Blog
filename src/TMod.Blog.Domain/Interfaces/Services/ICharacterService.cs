using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Common;

namespace TMod.Blog.Domain.Interfaces.Services
{
    /// <summary>
    /// 字符处理服务
    /// </summary>
    public interface ICharacterService
    {
        /// <summary>
        /// 把汉字转换为拼音
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="options">转换选项</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> ParseCharacterToSpellAsync(string? input, ChineseCharacterToSpellOptions? options = null,CancellationToken cancellationToken = default);
    }
}
