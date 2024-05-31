using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models;

namespace TMod.Blog.Data.Repositories.Implements
{
    internal class CategoryRepository : BaseIntKeyRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(BlogContext blogContext, ILoggerFactory loggerFactory) : base(blogContext, loggerFactory)
        {
        }

        public Category? GetCategoryByCategoryName(string category)
        {
            Category? meta = base.BlogContext.Categories.FirstOrDefault(p=>p.Category1 == category);
            return meta;
        }
    }
}
