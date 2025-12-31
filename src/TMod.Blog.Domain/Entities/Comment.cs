using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Entities
{
    public class Comment : BaseEntity<Guid>, ISharable,IFavoriteable
    {
        public required Guid ArticleId { get; set; }

        public Article Article { get; set; } = default!;

        [MaxLength(64)]
        public required string AuthorName { get; set; }

        [MaxLength(128)]
        public required string AuthorEmail { get; set; }

        public required string Content { get; set; }
        public bool IsShareEnabled { get; set; } = true;

        public Guid? ParentId { get; set; }

        public Comment? Parent { get; set; }

        public virtual ICollection<Comment> Replies { get; set; } = [];

        [MaxLength(64)]
        public string? AuthorIp { get; set; }
    }
}
