using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Web.Models;
using TMod.Blog.Web.Models.Articles;

namespace TMod.Blog.Web.Interactive.Abstraction
{
    public  interface IArticleApi
    {
        Task<PagingResult<ArticleViewModel?>> GetAllArticleByPaging(int pageSize, QueryArticleFilterModel? filterModel, int pageIndex = 1);

        Task<ArticleViewModel?> UpdateArticleCommentIsEnabledAsync(Guid articleId, bool isEnabled);

        Task<bool> BatchUpdateArticleCommentIsEnabledAsync(Dictionary<Guid, bool> dic);
    }
}
