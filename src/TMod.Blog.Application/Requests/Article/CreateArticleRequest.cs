using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application.Requests.Article
{
    public record CreateArticleRequest
    {
        public required string Title { get; set; }

        public string? Content { get; set; }

        public Guid? CategoryId { get; set; }

        public List<string> Tags { get; set; } = [];

        public bool IsCommentEnabled { get; set; }

        public bool IsShareEnabled { get; set; }
    }
}
