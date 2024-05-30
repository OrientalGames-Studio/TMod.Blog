using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models;

namespace TMod.Blog.Data.Repositories.Implements
{
    internal class ArticleContentRepository : BaseIntKeyRepository<ArticleContent>, IArticleContentRepository
    {
        public ArticleContentRepository(BlogContext blogContext, ILoggerFactory loggerFactory) : base(blogContext, loggerFactory)
        {
        }

        public ArticleContent? GetArticleContentByArticleId(Guid articleId)
        {
            Article? article = base.BlogContext.Articles.FirstOrDefault(p=>p.Id == articleId);
            if(article is null )
            {
                return null;
            }
            ArticleContent? articleContent = base.BlogContext.ArticleContents.FirstOrDefault(p=>p.ArticleId == articleId);
            return articleContent;
        }
    }
}
