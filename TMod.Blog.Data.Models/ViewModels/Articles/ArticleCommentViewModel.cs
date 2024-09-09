using System.Text.Json.Serialization;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models.ViewModels.Articles
{
    public sealed class ArticleCommentViewModel : IGuidKey, IVersionControl, ICreate, IUpdate, IRemove, IEdit, INotifiable, IKey<Guid>
    {
        /// <summary>
        /// 评论主键标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 文章外键标识
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// 父级评论标识
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 评论内容的二进制数据
        /// </summary>
        public byte[] ByteContent { get; set; } = [];

        /// <summary>
        /// 评论显示状态
        /// </summary>
        public bool CommentIsVisible { get; set; }

        /// <summary>
        /// 是否在收到回复时发送通知
        /// </summary>
        public bool NotifitionWhenReply { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsRemove { get; set; }

        /// <summary>
        /// 评论版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 评论者昵称
        /// </summary>
        public string CreateUserName { get; set; } = string.Empty;

        /// <summary>
        /// 评论者邮箱，用于通知有新回复
        /// </summary>
        public string CreateUserEmail { get; set; } = string.Empty;

        /// <summary>
        /// 评论发表时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 上次编辑评论时间
        /// </summary>
        public DateTime? LastEditDate { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? RemoveDate { get; set; }

        /// <summary>
        /// 评论回复数量
        /// </summary>
        public int RepliesCount => Replies.Count();

        /// <summary>
        /// 父评论
        /// </summary>
        [JsonIgnore]
        public ArticleCommentViewModel? ParentComment { get; set; }
        
        /// <summary>
        /// 回复的评论
        /// </summary>
        public ICollection<ArticleCommentViewModel> Replies { get; set; } = [];

        public static implicit operator ArticleComment?(ArticleCommentViewModel? model)
        {
            return ConvertToArticleComment(model, new HashSet<ArticleComment>());
        }

        private static ArticleComment? ConvertToArticleComment(ArticleCommentViewModel? model, HashSet<ArticleComment> processedIds)
        {
            if ( model is null )
            {
                return null;
            }
            if(processedIds.Any(p=>p.Id == model.Id) )
            {
                return processedIds.FirstOrDefault(p => p.Id == model.Id);
            }

            //processedIds.Add(model.Id);

            ArticleComment comment = new ArticleComment
            {
                Id = model.Id,                
            };
            processedIds.Add(comment);
            comment.ArticleId = model.ArticleId;
            comment.ParentId = model.ParentId;
            comment.Content = model.Content;
            comment.ByteContent = model.ByteContent;
            comment.State = model.CommentIsVisible ? ( short )0 : ( short )1;
            comment.NotifitionWhenReply = model.NotifitionWhenReply;
            comment.IsRemove = model.IsRemove;
            comment.Version = model.Version;
            comment.CreateUserName = model.CreateUserName;
            comment.CreateUserEmail = model.CreateUserEmail;
            comment.CreateDate = model.CreateDate;
            comment.UpdateDate = model.UpdateDate;
            comment.LastEditDate = model.LastEditDate;
            comment.RemoveDate = model.RemoveDate;
            comment.ParentComment = ConvertToArticleComment(model.ParentComment, processedIds);
            comment.Replies = model.Replies.Select(reply => ConvertToArticleComment(reply, processedIds)).ToList();
            return comment;
        }


        public static implicit operator ArticleCommentViewModel?(ArticleComment? comment)
        {
            return ConvertToArticleCommentViewModel(comment, new HashSet<ArticleCommentViewModel>());
        }

        private static ArticleCommentViewModel? ConvertToArticleCommentViewModel(ArticleComment? comment,HashSet<ArticleCommentViewModel> processedIds)
        {
            if(comment is null)
            {
                return null;
            }
            if(processedIds.Any(p=>p.Id == comment.Id) )
            {
                return processedIds.FirstOrDefault(p => p.Id == comment.Id);
            }
            //processedIds.Add(comment.Id);
            ArticleCommentViewModel viewModel = new ArticleCommentViewModel()
            {
                Id = comment.Id,
            };
            processedIds.Add(viewModel);
            viewModel.ArticleId = comment.ArticleId;
            viewModel.ParentId = comment.ParentId;
            viewModel.Content = comment.Content;
            viewModel.ByteContent = comment.ByteContent;
            viewModel.CommentIsVisible = comment.State == 0;
            viewModel.NotifitionWhenReply = comment.NotifitionWhenReply;
            viewModel.IsRemove = comment.IsRemove;
            viewModel.Version = comment.Version;
            viewModel.CreateUserName = comment.CreateUserName;
            viewModel.CreateUserEmail = comment.CreateUserEmail;
            viewModel.CreateDate = comment.CreateDate;
            viewModel.UpdateDate = comment.UpdateDate;
            viewModel.RemoveDate = comment.RemoveDate;
            viewModel.LastEditDate = comment.LastEditDate;
            viewModel.ParentComment = ConvertToArticleCommentViewModel(comment.ParentComment, processedIds);
            viewModel.Replies = ( from reply in comment.Replies select ConvertToArticleCommentViewModel(reply, processedIds) ).ToList();
            return viewModel;
        }
    }
}