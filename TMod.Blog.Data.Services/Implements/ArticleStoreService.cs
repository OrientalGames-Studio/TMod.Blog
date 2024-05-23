using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Repositories;

namespace TMod.Blog.Data.Services.Implements
{
    internal class ArticleStoreService : IArticleStoreService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IArticleArchiveRepository _articleArchiveRepository;
        private readonly IArticleCategoryRepository _articleCategoryRepository;
        private readonly IArticleCommentRepository _articleCommentRepository;
        private readonly IArticleContentRepository _articleContentRepository;
        private readonly IArticleTagRepository _articleTagRepository;
        private readonly ILogger<ArticleStoreService> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly BlogContext _blogContext;

        public ArticleStoreService(IArticleRepository articleRepository, IArticleArchiveRepository articleArchiveRepository, IArticleCategoryRepository articleCategoryRepository, IArticleCommentRepository articleCommentRepository, IArticleContentRepository articleContentRepository, IArticleTagRepository articleTagRepository, ILogger<ArticleStoreService> logger, ICategoryRepository categoryRepository, BlogContext blogContext)
        {
            _articleRepository = articleRepository;
            _articleArchiveRepository = articleArchiveRepository;
            _articleCategoryRepository = articleCategoryRepository;
            _articleCommentRepository = articleCommentRepository;
            _articleContentRepository = articleContentRepository;
            _articleTagRepository = articleTagRepository;
            _logger = logger;
            _categoryRepository = categoryRepository;
            _blogContext = blogContext;
        }
    }
}
