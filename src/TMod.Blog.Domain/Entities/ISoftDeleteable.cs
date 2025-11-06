using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Entities
{
    public interface ISoftDeleteable
    {
        bool IsDeleted { get; set; }

        DateTime? DeleteDate { get; set; }
    }
}
