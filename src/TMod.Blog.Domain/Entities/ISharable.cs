using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Entities
{
    /// <summary>
    /// 可分享实体接口
    /// </summary>
    public interface ISharable
    {
        /// <summary>
        /// 是否允许分享
        /// </summary>
        bool IsShareEnabled { get; set; }
    }
}
