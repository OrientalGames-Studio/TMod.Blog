using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application.Requests.Article
{
    public record PatchArticleCategoryRequest
    {
        public Guid CategoryId { get; set; }
    }
}
