using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application.Requests.Category
{
    public record CreateCategoryRequest
    {
        public required string Name { get; set; }

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }
    }
}
