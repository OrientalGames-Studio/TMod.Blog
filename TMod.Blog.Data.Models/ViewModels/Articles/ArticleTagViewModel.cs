using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models.ViewModels.Articles
{
    public sealed class ArticleTagViewModel : IIntKey, IKey<int>
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 文章外键标识
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// 文章标签
        /// </summary>
        public string Tag { get; set; } = null!;

        /// <summary>
        /// 编辑时，标签是否标记为删除
        /// </summary>
        public bool IsRemove { get; set; } = false;

        public static implicit operator ArticleTag?(ArticleTagViewModel? viewModel)
        {
            if(viewModel is null )
            {
                return null;
            }
            ArticleTag tag = new ArticleTag()
            {
                Id = viewModel.Id,
                ArticleId = viewModel.ArticleId,
                Tag = viewModel.Tag
            };
            return tag;
        }

        public static implicit operator ArticleTagViewModel?(ArticleTag? tag)
        {
            if(tag is null )
            {
                return null;
            }
            ArticleTagViewModel viewModel = new ArticleTagViewModel()
            {
                Id = tag.Id,
                ArticleId = tag.ArticleId,
                Tag = tag.Tag
            };
            return viewModel;
        }
    }
}
