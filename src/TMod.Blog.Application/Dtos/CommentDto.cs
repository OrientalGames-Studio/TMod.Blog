using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application.Dtos
{
    public record CommentDto
    {
        public Guid Id { get; set; }

        public required string AuthorName { get; set; }

        public required string AuthorEmail { get; set; }

        public required string Content { get; set; }

        public bool IsShareEnabled { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? ArticleId { get; set; }

        public bool IsDeleted { get; set; }

        public string? AuthorIp { get; set; }

        public DateOnly CommentDate { get; set; }

        public TimeOnly CommentTime { get; set; }
    }
}
