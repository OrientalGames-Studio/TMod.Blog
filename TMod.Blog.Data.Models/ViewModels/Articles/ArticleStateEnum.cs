﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Models.ViewModels.Articles
{
    [Flags]
    public enum ArticleStateEnum : short
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        Draft = 1,

        /// <summary>
        /// 已发布
        /// </summary>
        [Description("已发布")]
        Published = 2,

        /// <summary>
        /// 已隐藏
        /// </summary>
        [Description("已隐藏")]
        Hidden = 4
    }
}
