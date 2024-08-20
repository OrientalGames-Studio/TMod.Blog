using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Web.Services.Abstraction
{
    public interface ITagService
    {
        Task<IEnumerable<string?>> GetTagsAsync();
    }
}
