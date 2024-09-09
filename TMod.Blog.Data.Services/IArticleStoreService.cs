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

        Task<Guid> UpdateArticleStateAsync(Guid articleId, ArticleStateEnum state);

        Task<bool> BatchUpdateArticleStateAsync(Dictionary<Guid, ArticleStateEnum> dic);

        Task<Guid> UpdateArticleCommentEnabledFlagAsync(Guid articleId, bool isEnabled);

        Task<bool> BatchUpdateArticleCommentEnabledFlagAsync(Dictionary<Guid, bool> dic);

        Task RemoveArticleByIdAsync(Guid articleId);

        Task BatchRemoveArticleAsync(BatchRemoveArticleModel model);

        IEnumerable<ArticleCommentViewModel?> PaingLoadArticleComments(Guid articleId, int pageIndex, int pageSize, out int totalDataCount, out int totalPageCount, Guid? commentId = null,bool showAll = false);

        Task<int> CountCommentReplyCountAsync(Guid articleId, Guid commentId);

        Task<ArticleCommentViewModel?> ReplyCommentAsync(ReplyCommentModel model);
    }
}
