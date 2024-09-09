using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Web.Core.Shared.EventCallbackArgs;
using TMod.Blog.Web.Models;
using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Core.Components.Articles
{
    public partial class ArticlePreview : ComponentBase
    {

        private ArticleContentViewModel? _articleContent;

        private PagingResult<ArticleCommentViewModel?>? _articleComments;
        private int _pageIndex = 1;
        private int _pageSize = 1;

        [Parameter]
        public ArticleViewModel? Article { get; set; }

        [Parameter]
        public bool IsAdmin { get; set; }

        [Inject]
        public IArticleService? ArticleService { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            if ( Article is not null )
            {
                _articleContent = await ArticleService!.LoadArticleContentAsync(Article.Id);
                await LoadCommentsAsync();
            }
        }

        private async Task LoadCommentsAsync()
        {
            if(Article is not null )
            {
                _articleComments = await ArticleService!.GetArticleCommentByPaging(_pageSize, Article.Id, null, _pageIndex, IsAdmin);
            }
        }

        private async Task OnPageIndexChangedAsync(int pageIndex)
        {
            if(_pageIndex != pageIndex )
            {
                _pageIndex = pageIndex;
                await LoadDataAsync();
            }
        }

        private async Task OnCommentCommitedAsync(CommentCommitEventCallbackArgs args)
        {
            if(Article is not null )
            {
                await ArticleService!.CommitReplyAsync(Article.Id, args.ReplyContent, args.Email, args.NickName, args.NotifyWhenReply, args.ReplyCommentId);
                await LoadCommentsAsync();
            }
        }
    }
}
