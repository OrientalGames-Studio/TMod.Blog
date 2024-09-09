using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Primitives;

using System.Text;

using TMod.Blog.Data.Models.DTO.Articles;

namespace TMod.Blog.Api.Tools.ModelBinders
{
    internal class ReplyCommentModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext, nameof(bindingContext));
            ReplyCommentModel? comment = new ReplyCommentModel();
            if(!bindingContext.HttpContext.Request.RouteValues.TryGetValue("articleId",out object? articleIdObj) || !Guid.TryParse(articleIdObj?.ToString(),out Guid articleId))
            {
                articleId = Guid.Empty;
                bindingContext.ModelState.TryAddModelError("articleId", "文章编号不允许为空");
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }
            comment.ArticleId = articleId;
            if(bindingContext.HttpContext.Request.RouteValues.TryGetValue("commentId",out object? parentCommentIdObj) && Guid.TryParse(parentCommentIdObj?.ToString(),out Guid parentCommentId) )
            {
                comment.ParentCommentId = parentCommentId;
            }
            if ( bindingContext.HttpContext.Request.HasFormContentType )
            {
                bool hasModelStateError = false;
                IFormCollection? form = bindingContext.HttpContext.Request.Form;
                if(!form.TryGetValue("email",out StringValues email) || string.IsNullOrWhiteSpace(email))
                {
                    bindingContext.ModelState.TryAddModelError("email", "评论者邮箱不允许为空！");
                    hasModelStateError = true;
                }
                else
                {
                    comment.Email = email;
                }
                if(!form.TryGetValue("nickName",out StringValues nickName) || string.IsNullOrWhiteSpace(nickName))
                {
                    bindingContext.ModelState.TryAddModelError("nickName", "评论者昵称不允许为空！");
                    hasModelStateError = true;
                }
                else
                {
                    comment.NickName = nickName;
                }
                if(!form.TryGetValue("notifyNewReply",out StringValues notifyNewReply) )
                {
                    comment.NotifyNewReply = false;
                }
                else
                {
                    if(bool.TryParse(notifyNewReply,out bool bNotifyNewReply) )
                    {
                        comment.NotifyNewReply = bNotifyNewReply;
                    }
                    else
                    {
                        comment.NotifyNewReply = false;
                    }
                }
                if(!form.TryGetValue("comment",out StringValues commentContent) || string.IsNullOrWhiteSpace(commentContent))
                {
                    bindingContext.ModelState.TryAddModelError("comment", "评论不允许为空！");
                    hasModelStateError = true;
                }
                if ( hasModelStateError )
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
            else if ( bindingContext.HttpContext.Request.HasJsonContentType() )
            {
                comment = await bindingContext.HttpContext.Request.ReadFromJsonAsync<ReplyCommentModel>();
                if(comment is null )
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
            else
            {
                using ( MemoryStream ms = new MemoryStream() )
                {
                    await bindingContext.HttpContext.Request.BodyReader.CopyToAsync(ms);
                    var body = Encoding.UTF8.GetString(ms.ToArray());
                    ;
                    // TODO: 二进制流数据解析
                }
            }
        }
    }

    internal class ReplyCommentModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            if(context.Metadata.ModelType == typeof(ReplyCommentModel) )
            {
                return new BinderTypeModelBinder(typeof(ReplyCommentModelBinder));
            }
            return null;
        }
    }
}
