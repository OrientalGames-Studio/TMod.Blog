using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application.Requests.Comment
{
    public record UpdateCommentRequest
    {
        public required string AuthorEmail { get; set; }

        public required string AuthorName { get; set; }

        public required string Content { get; set; }
    }
}
