using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Application.Dtos
{
    public record ArticleDto
    {
        public Guid Id { get;set; }

        public string Title { get; set; } = default!;

        public string? Content { get; set; }

        public string? Slug { get; set; }

        public ArticleStatusEnum Status { get; set; }

        public bool IsShareEnabled { get; set; }

        public bool IsCommentEnabled { get; set; }

        public bool IsDeleted {  get; set; }

        public string? CategoryName { get; set; }

        public List<string> Tags { get; set; } = [];

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public DateTime? DeleteDate { get; set; }
    }
}
