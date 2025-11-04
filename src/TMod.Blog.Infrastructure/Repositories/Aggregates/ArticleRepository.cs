using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Domain.Specifications;
using TMod.Blog.Infrastructure.Contextes;
using TMod.Blog.Infrastructure.Specifications;

namespace TMod.Blog.Infrastructure.Repositories.Aggregates
{
    internal class ArticleRepository(TMod_Blog_RW_Context context) : BlogRepository<Article, Guid>(context), IArticleRepository
    {
        public async Task<Article?> GetArticleWithDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            ISpecification<Article> getDetailSpecification = ArticleSpecification.CreateGetDetail(id);
            return await GetEntityAsync(getDetailSpecification,cancellationToken);
        }

        public async Task<IReadOnlyList<Article>> PagingArticleByCategoryAsync(Guid categoryId,string? keyword = null, ArticleStatusEnum articleStatus = ArticleStatusEnum.Draft| ArticleStatusEnum.Published| ArticleStatusEnum.Unpublished , int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            int skip = (Math.Max(1,pageIndex) - 1) * pageSize;
            ISpecification<Article> pagingArticleSpecification = ArticleSpecification.CreatePagingByCategoryId(categoryId,skip,pageSize);
            return await GetAllEntitiesAsync(pagingArticleSpecification,cancellationToken);
        }
    }
}
