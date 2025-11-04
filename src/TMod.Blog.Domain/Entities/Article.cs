using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Entities
{
    public class Article:BaseEntity<Guid>
    {
        [MaxLength(128)]
        public required string Title { get; set; }

        public required string Content { get; set; }

        [MaxLength(128)]
        public required string? Slug { get; set; }

        public ArticleStatusEnum Status { get; set; }

        public bool IsShareEnabled { get; set; } = true;

        public bool IsCommentEnabled { get; set; } = true;

        public Guid? CategoryId { get; set; }

        public Category? Category { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = [];

        public virtual ICollection<Comment> Comments { get; set; } = [];
    }
}
