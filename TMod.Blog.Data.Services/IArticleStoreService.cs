using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.StoreServices;
using TMod.Blog.Data.Models.DTO.Articles;
using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Data.Models.ViewModels.Categories;

namespace TMod.Blog.Data.Services
{
    public interface IArticleStoreService : IStoreService,IPagingStoreService<ArticleViewModel>
    {
        IAsyncEnumerable<ArticleViewModel?> GetArticlesAsync();

        Task<ArticleViewModel?> GetArticleByIdAsync(Guid id);

        ArticleContentViewModel GetArticleContentByArticleId(Guid articleId);

        IEnumerable<ArticleArchiveViewModel> GetArticleArchivesByArticleId(Guid articleId);

        ArticleArchiveContentViewModel GetArticleArchiveContent(Guid articleId, Guid archiveId);

        Task<Guid> CreateArticleAsync(AddArticleModel articleModel, IEnumerable<AddArticleArchiveModel> archiveModels);

        Task<Guid> UpdateArticleAsync(Guid articleId, ArticleViewModel article, ArticleContentViewModel articleContent, IEnumerable<CategoryViewModel> deletedCategories, IEnumerable<CategoryViewModel> addedCategories, IEnumerable<ArticleTagViewModel> deletedTags, IEnumerable<ArticleTagViewModel> addedTags, IEnumerable<ArticleArchiveViewModel> deletedArchives, IEnumerable<AddArticleArchiveModel> addedArchives);
    }
}
