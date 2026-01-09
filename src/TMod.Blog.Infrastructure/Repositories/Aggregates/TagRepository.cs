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
    internal class TagRepository(TMod_Blog_RW_Context context) : BlogRepository<Tag, Guid>(context), ITagRepository
    {
        public async Task<Tag?> GetByNameAsync(string? name, CancellationToken cancellationToken = default)
        {
            if ( string.IsNullOrWhiteSpace(name) )
            {
                return null;
            }
            ISpecification<Tag> specification = TagSpecification.CreateGetByName(name);
            return await GetEntityAsync(specification,cancellationToken);
        }

        public async Task<IReadOnlyList<Article>> PagingArticleByTagAsync(Guid tagId, string? keyword = null, ArticleStatusEnum articleStatus = ArticleStatusEnum.Draft | ArticleStatusEnum.Published | ArticleStatusEnum.Unpublished, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            Tag? tag = await GetEntityByIdAsync(tagId,true,cancellationToken);
            if(tag == null || tag.Articles is null || tag.Articles.Count == 0)
            {
                return [];
            }
            int skip = (Math.Max(1,pageIndex) - 1) * pageSize;
            ISpecification<Article> specification = ArticleSpecification.CreatePaging(skip,pageSize,keyword,articleStatus);
            return await QueryAllAsync<Article>(tag.Articles.AsQueryable(),specification,cancellationToken);
        }

        public async Task<IReadOnlyList<Tag>> PagingTagsAsync(string? keyword = null, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            int skip = (Math.Max(1,pageIndex) - 1) * pageSize;
            ISpecification<Tag> specification = TagSpecification.CreatePaging(keyword,skip,pageIndex);
            return await GetAllEntitiesAsync(specification,cancellationToken);
        }
    }
}
