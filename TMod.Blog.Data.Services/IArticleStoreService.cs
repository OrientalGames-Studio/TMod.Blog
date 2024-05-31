using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.StoreServices;
using TMod.Blog.Data.Models.DTO.Articles;
using TMod.Blog.Data.Models.ViewModels.Articles;

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
    }
}
