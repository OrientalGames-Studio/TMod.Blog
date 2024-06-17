using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Web.Services.Abstraction
{
    public interface IIconPathProviderService
    {
        string BrandIcon { get; }

        string LightModeIcon { get; }

        string DarkModeIcon { get; }
    }
}
