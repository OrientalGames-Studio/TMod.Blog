using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application.Requests.Category
{
    public record PatchCategoryParentRequest
    {
        public Guid? ParentId { get; set; }
    }
}
