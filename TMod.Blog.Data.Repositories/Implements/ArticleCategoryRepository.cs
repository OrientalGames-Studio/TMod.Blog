using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models;

namespace TMod.Blog.Data.Repositories.Implements
{
    internal class ArticleCategoryRepository : BaseIntKeyRepository<ArticleCategory>, IArticleCategoryRepository
    {
        public ArticleCategoryRepository(BlogContext blogContext, ILoggerFactory loggerFactory) : base(blogContext, loggerFactory)
        {
        }

        public IEnumerable<ArticleCategory> GetArticleCategories(Guid articleId)
        {
            Article? article = base.BlogContext.Articles.FirstOrDefault(p=>p.Id == articleId);
            if(article is null )
            {
                yield break;
            }
            IEnumerable<ArticleCategory> categories = base.BlogContext.ArticleCategories.Include(p=>p.Category).Where(p=>p.ArticleId == articleId);
            foreach (ArticleCategory category in categories)
            {
                yield return category;
            }
        }

        public ArticleCategory? GetArticleCategoryById(Guid articleId, int categoryId)
        {
            Article? article = base.BlogContext.Articles.FirstOrDefault(p=>p.Id == articleId);
            if(article is null )
            {
                return null;
            }
            return base.BlogContext.ArticleCategories.FirstOrDefault(p=>p.ArticleId == article.Id && p.CategoryId == categoryId);
        }
    }
}
