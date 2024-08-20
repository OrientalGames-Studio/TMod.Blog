using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Abstractions.StoreServices;
using TMod.Blog.Data.Models.ViewModels.Articles;

namespace TMod.Blog.Data.Services
{
    public interface IArticleTagsStoreService:IStoreService,IPagingStoreService<ArticleTagViewModel>
    {
        Task<Guid> AddTagsToArticleAsync(Guid articleId, IEnumerable<string> tags);

        Task RemoveTagsFromArticleAsync(Guid articleId, IEnumerable<string> tags);

        IEnumerable<string> GetTags();
    }
}
