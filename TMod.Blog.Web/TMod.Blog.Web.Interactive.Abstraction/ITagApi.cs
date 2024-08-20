using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Web.Interactive.Abstraction
{
    public interface ITagApi
    {
        Task<IEnumerable<string?>> GetTagsAsync();
    }
}
