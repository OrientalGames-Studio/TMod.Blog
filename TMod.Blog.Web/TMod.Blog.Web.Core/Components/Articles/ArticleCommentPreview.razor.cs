using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Web.Core.Shared.EventCallbackArgs;
using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Core.Components.Articles
{
    public partial class ArticleCommentPreview:ComponentBase
    {
        private ConcurrentDictionary<Guid,bool> _showCommentsDic = new ConcurrentDictionary<Guid, bool>();
        private MudTextField<string> _emailField;
        private MudTextField<string> _nickNameField;
        private string? _email;
        private string? _nickName;
        private string? _commentValue;
        private bool _notifyWhenReply;

        [Parameter]
        public HashSet<ArticleCommentViewModel?>? Comments { get; set; }

        [Parameter]
        public ArticleCommentViewModel? CurrentComment { get; set; }

        [Parameter]
        public bool IsAdmin { get; set; }

        [Parameter]
        public EventCallback<CommentCommitEventCallbackArgs> OnCommentCommitedEvent { get; set; }

        private string GetFormattedReplyDate(DateTime? date)
        {
            DateTime today = DateTime.Now;
            if(date is null || !date.HasValue )
            {
                return "刚刚";
            }
            TimeSpan timeDifference = today - date.Value;

            if ( timeDifference.TotalSeconds < 30 )
            {
                return "刚刚";
            }
            else if ( timeDifference.TotalSeconds < 60 )
            {
                return $"{( int )timeDifference.TotalSeconds} 秒前";
            }
            else if ( timeDifference.TotalMinutes < 60 )
            {
                return $"{( int )timeDifference.TotalMinutes} 分钟前";
            }
            else if ( timeDifference.TotalHours < 24 )
            {
                return $"{( int )timeDifference.TotalHours} 小时前";
            }
            else if ( timeDifference.TotalDays <= 7 )
            {
                return $"{( int )timeDifference.TotalDays} 天前";
            }
            else
            {
                return date.Value.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if ( Comments is not null )
            {
                _showCommentsDic.Clear();
                foreach ( var comment in Comments )
                {
                    if ( comment is not null )
                    {
                        _showCommentsDic.AddOrUpdate(comment.Id, false, (id, val) => false);
                    }
                }
            }
        }

        private async Task OnCommentCommitedAsync(CommentCommitEventCallbackArgs args) => await OnCommentCommitedEvent.InvokeAsync(args);

        private async Task OnCommitButtonClickAsync(MouseEventArgs args)
        {
            if(args.Detail > 1 )
            {
                return;
            }
            await _emailField.Validate();
            await _nickNameField.Validate();
            if(_emailField.Error || _nickNameField.Error )
            {
                return;
            }
            string? tempCommentValue = _commentValue?.TrimEnd();
            if ( string.IsNullOrWhiteSpace(tempCommentValue) )
            {
                return;
            }
            CommentCommitEventCallbackArgs commentCommitEventCallbackArgs = new CommentCommitEventCallbackArgs()
            {
                ReplyCommentId = CurrentComment?.Id??null,
                Email = _email,
                NickName = _nickName,
                ReplyContent = tempCommentValue,
                NotifyWhenReply = _notifyWhenReply
            };
            await OnCommentCommitedEvent.InvokeAsync(commentCommitEventCallbackArgs);
        }
    }
}
