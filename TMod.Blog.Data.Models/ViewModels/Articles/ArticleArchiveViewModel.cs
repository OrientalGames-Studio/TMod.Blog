using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models.ViewModels.Articles
{
    internal class ArticleArchiveViewModel : IGuidKey, IKey<Guid>
    {
        /// <summary>
        /// 附件主键标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 文章外键标识
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// 附件文件数据
        /// </summary>
        public byte[] ArchiveContent { get; set; } = null!;
    }
}
