﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.Repositories;
using TMod.Blog.Data.Models;

namespace TMod.Blog.Data.Repositories
{
    public interface IArticleTagRepository : IIntKeyRepository<ArticleTag>
    {
        IEnumerable<ArticleTag> GetArticleTags(Guid articleId);

        ArticleTag? GetArticleTagByTag(Guid articleId, string tag);

        IEnumerable<ArticleTagsView> GetTags();
    }
}
