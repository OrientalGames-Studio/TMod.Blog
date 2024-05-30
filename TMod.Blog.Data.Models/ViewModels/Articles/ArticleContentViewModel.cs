using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models.ViewModels.Articles
{
    public class ArticleContentViewModel : IIntKey, IKey<int>
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

        public static implicit operator ArticleContent? (ArticleContentViewModel? viewModel)
        {
            if(viewModel is null )
            {
                return null;
            }
            ArticleContent articleContent = new ArticleContent()
            {
                Id = viewModel.Id,
                ArticleId = viewModel.ArticleId,
                Content = viewModel.Content,
            };
            return articleContent;
        }

        public static implicit operator ArticleContentViewModel? (ArticleContent? articleContent)
        {
            if(articleContent is null )
            {
                return null;
            }
            ArticleContentViewModel viewModel = new ArticleContentViewModel()
            {
                Id = articleContent.Id,
                ArticleId = articleContent.ArticleId,
                Content = articleContent.Content
            };
            return viewModel;
        }
    }
}
