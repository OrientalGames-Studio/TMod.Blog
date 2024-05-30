using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models.ViewModels.Articles
{
    internal class ArticleContentViewModel : IIntKey, IKey<int>
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 文章编号外键
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// 文章正文内容
        /// </summary>
        public string Content { get; set; } = null!;
    }
}
