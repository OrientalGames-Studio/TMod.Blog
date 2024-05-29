using System.ComponentModel.DataAnnotations;

using TMod.Blog.Data.Interfaces;
using TMod.Blog.Data.Models.ViewModels.Categories;

namespace TMod.Blog.Data.Models.ViewModels.Articles
{
    public sealed class ArticleViewModel : IVersionControl, ICreate, IUpdate, IRemove, IEdit, IGuidKey, IKey<Guid>
    {
        /// <summary>
        /// Uid 主键标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        [StringLength(60)]
        public string Title { get; set; } = null!;

        /// <summary>
        /// 文章前150个字内容节选
        /// </summary>
        [StringLength(150)]
        public string? Snapshot { get; set; }

        /// <summary>
        /// 文章状态（0：草稿，1：已发布，2：已隐藏）
        /// </summary>
        public ArticleStateEnum State { get; set; }

        /// <summary>
        /// 文章是否允许评论
        /// </summary>
        public bool IsCommentEnabled { get; set; }

        /// <summary>
        /// 最后一次编辑文章日期
        /// </summary>
        public DateTime? LastEditDate { get; set; }

        /// <summary>
        /// 文章首次发表日期
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsRemove { get; set; }

        /// <summary>
        /// 文章版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 文章创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 文章修改日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 文章删除日期
        /// </summary>
        public DateTime? RemoveDate { get; set; }

        /// <summary>
        /// 文章标签
        /// </summary>
        public List<ArticleTagViewModel> Tags { get; set; } = new List<ArticleTagViewModel>();

        /// <summary>
        /// 文章分类
        /// </summary>
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();

        public static implicit operator Article? (ArticleViewModel? viewModel)
        {
            if(viewModel is null )
            {
                return null;
            }
            Article article = new Article();
            article.Id = viewModel.Id;
            article.Title = viewModel.Title;
            article.Snapshot = viewModel.Snapshot;
            article.State = (short)viewModel.State;
            article.IsCommentEnabled = viewModel.IsCommentEnabled;
            article.LastEditDate = viewModel.LastEditDate;
            article.PublishDate = viewModel.PublishDate;
            article.RemoveDate = viewModel.RemoveDate;
            article.Version = viewModel.Version;
            article.IsRemove = viewModel.IsRemove;
            article.CreateDate = viewModel.CreateDate;
            article.UpdateDate = viewModel.UpdateDate;
            article.ArticleTags = viewModel.Tags.Select(p=>(ArticleTag)p!).ToList();
            article.ArticleCategories = viewModel.Categories.Select(p => new ArticleCategory() { ArticleId = article.Id, CategoryId = p.Id }).ToList();
            return article;
        }

        public static implicit operator ArticleViewModel? (Article? article)
        {
            if(article is null )
            {
                return null;
            }
            ArticleViewModel viewModel = new ArticleViewModel();
            viewModel.Id = article.Id;
            viewModel.Title = article.Title;
            viewModel.Snapshot = article.Snapshot;
            viewModel.State = Enum.Parse<ArticleStateEnum>(article.State.ToString());
            viewModel.IsCommentEnabled = article.IsCommentEnabled;
            viewModel.LastEditDate = article.LastEditDate;
            viewModel.PublishDate = article.PublishDate;
            viewModel.RemoveDate = article.RemoveDate;
            viewModel.Version = article.Version;
            viewModel.IsRemove = article.IsRemove;
            viewModel.CreateDate = article.CreateDate;
            viewModel.UpdateDate = article.UpdateDate;
            viewModel.Tags = article.ArticleTags.Select(p => ( ArticleTagViewModel )p!).ToList();
            viewModel.Categories = article.ArticleCategories.Select(p => (CategoryViewModel)p.Category!).ToList();
            return viewModel;
        }
    }
}
