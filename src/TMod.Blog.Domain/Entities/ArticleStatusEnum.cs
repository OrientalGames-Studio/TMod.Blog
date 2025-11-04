using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Entities
{
    [Flags]
    public enum ArticleStatusEnum
    {
        Draft = 2,
        Published = 4,
        Unpublished = 8,
    }
}
