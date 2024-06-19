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

        string EditIcon { get; }

        string ListIcon { get; }

        string PreviewIcon { get; }

        string ConfigurationIcon { get; }

        string ReportIcon { get; }

        string NavMenu_HomeIcon { get; }

        string NavMenu_Admin_DashboardIcon { get; }

        string NavMenu_Admin_ArticlesIcon { get; }

        string NavMenu_Admin_CategoryIcon { get; }
    }
}
