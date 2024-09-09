using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Models.DTO.Articles
{
    public sealed class ReplyCommentModel
    {
        [Required]
        public Guid ArticleId { get; set; }

        public Guid? ParentCommentId { get; set; }

        [Required]
        [StringLength(50,MinimumLength = 1)]
        public string? Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string? NickName { get; set; }

        public bool NotifyNewReply { get; set; }

        [Required]
        public string? Comment { get; set; }
    }
}
