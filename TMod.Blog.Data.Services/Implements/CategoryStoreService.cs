using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Repositories;

namespace TMod.Blog.Data.Services.Implements
{
    internal class CategoryStoreService : ICategoryStoreService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IArticleCategoryRepository _articleCategoryRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly ILogger<CategoryStoreService> _logger;

        public CategoryStoreService(ICategoryRepository categoryRepository, IArticleCategoryRepository articleCategoryRepository, IArticleRepository articleRepository, ILogger<CategoryStoreService> logger)
        {
            _categoryRepository = categoryRepository;
            _articleCategoryRepository = articleCategoryRepository;
            _articleRepository = articleRepository;
            _logger = logger;
        }
    }
}
