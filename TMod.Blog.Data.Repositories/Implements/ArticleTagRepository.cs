using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models;

namespace TMod.Blog.Data.Repositories.Implements
{
    internal class ArticleTagRepository : BaseIntKeyRepository<ArticleTag>, IArticleTagRepository
    {
        public ArticleTagRepository(BlogContext blogContext, ILoggerFactory loggerFactory) : base(blogContext, loggerFactory)
        {
        }

        public ArticleTag? GetArticleTagByTag(Guid articleId, string tag)
        {
            Article? article = base.BlogContext.Articles.FirstOrDefault(p=>p.Id == articleId);
            if ( article is null )
            {
                return null;
            }
            return base.BlogContext.ArticleTags.FirstOrDefault(p => p.ArticleId == articleId && p.Tag == tag);
        }

        public IEnumerable<ArticleTag> GetArticleTags(Guid articleId)
        {
            Article? article = base.BlogContext.Articles.FirstOrDefault(p=>p.Id == articleId);
            if(article is null )
            {
                yield break;
            }
            IEnumerable<ArticleTag> tags = base.BlogContext.ArticleTags.Where(p=>p.ArticleId ==  articleId);
            foreach (ArticleTag tag in tags)
            {
                yield return tag;
            }
        }

        public IEnumerable<ArticleTagsView> GetTags() => base.BlogContext.ArticleTagsViews;
    }
}
