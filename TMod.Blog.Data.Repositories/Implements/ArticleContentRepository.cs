﻿using Microsoft.Extensions.Logging;

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
    }
}
