using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Web.Models.Articles;
using TMod.Blog.Web.Models;

namespace TMod.Blog.Web.Services.Abstraction
{
    public interface IArticleService
    {
        Task<PagingResult<ArticleViewModel?>> GetAllArticleByPaging(int pageSize, QueryArticleFilterModel? filterModel, int pageIndex = 1);

        Task<ArticleViewModel?> UpdateArticleCommentIsEnabledAsync(Guid articleId, bool isEnabled);

        Task<bool> BatchUpdateArticleCommentIsEnabledAsync(Dictionary<Guid, bool> dic);

        Task<ArticleViewModel?> UpdateArticleStateAsync(Guid articleId, ArticleStateEnum articleState);

        Task<ArticleViewModel?> LoadArticleAsync(Guid articleId);

        Task<bool> BatchUpdateArticleStateAsync(Dictionary<Guid, ArticleStateEnum> articleStates);

        Task<bool> RemoveArticleAsync(Guid articleId);

        Task<bool> BatchRemoveArticleAsync(List<Guid> articleIds);

        Task<ArticleContentViewModel> LoadArticleContentAsync(Guid articleId);

        Task<PagingResult<ArticleCommentViewModel?>> GetArticleCommentByPaging(int pageSize, Guid articleId, Guid? parentId = null, int pageIndex = 1, bool showAll = false);

        Task CommitReplyAsync(Guid replyArticleId, string? replyContent, string? email, string? nickName, bool notifyWhenReply, Guid? replyCommentId = null);
    }
}
