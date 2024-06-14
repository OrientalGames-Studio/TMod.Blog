using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Web.Core.Components.Admin
{
    public partial class AdminAppBar:ComponentBase
    {
        private bool _isMenuOpen = true;

        [Parameter]
        public RenderFragment? Body { get; set; }
    }
}
