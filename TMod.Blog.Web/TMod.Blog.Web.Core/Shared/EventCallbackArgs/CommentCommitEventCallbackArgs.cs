using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Web.Core.Shared.EventCallbackArgs
{
    public sealed class CommentCommitEventCallbackArgs
    {
        public Guid? ReplyCommentId { get; set; }

        public string? ReplyContent { get; set; }

        public string? Email { get; set; }

        public string? NickName { get; set; }

        public bool NotifyWhenReply { get; set; }
    }
}
