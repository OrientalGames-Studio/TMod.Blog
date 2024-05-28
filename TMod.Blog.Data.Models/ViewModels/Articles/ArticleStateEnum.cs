using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Models.ViewModels.Articles
{
    public enum ArticleStateEnum : short
    {
        /// <summary>
        /// 草稿
        /// </summary>
        Draft = 0,

        /// <summary>
        /// 已发布
        /// </summary>
        Published = 1,

        /// <summary>
        /// 已隐藏
        /// </summary>
        Hidden = 2
    }
}
