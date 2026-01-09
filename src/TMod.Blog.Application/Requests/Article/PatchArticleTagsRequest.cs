using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application.Requests.Article
{
    public record PatchArticleTagsRequest
    {
        public List<string> Tags { get; set; } = [];

        public List<string> AddedTags { get; set; } = [];

        public List<string> RemovedTags { get; set; } = [];
    }
}
