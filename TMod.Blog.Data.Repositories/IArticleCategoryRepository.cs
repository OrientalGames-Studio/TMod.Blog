﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.Repositories;
using TMod.Blog.Data.Models;

namespace TMod.Blog.Data.Repositories
{
    public interface IArticleCategoryRepository:IIntKeyRepository<ArticleCategory>
    {
        IEnumerable<ArticleCategory> GetArticleCategories(Guid articleId);

        ArticleCategory? GetArticleCategoryById(Guid articleId, int categoryId);
    }
}
