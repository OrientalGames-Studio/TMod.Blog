using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Web.Models;

namespace TMod.Blog.Web.Services.Abstraction
{
    public interface INavMenuProviderService
    {
        IEnumerable<MenuItem> GetNavMenuItems();
    }
}
