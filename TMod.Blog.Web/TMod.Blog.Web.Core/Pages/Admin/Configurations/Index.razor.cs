using Microsoft.AspNetCore.Components;

using MudBlazor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Web.Core.Pages.Admin.Configurations
{
    public partial class Index:ComponentBase
    {
        private DateRange? _createDateRange;
        private string? _searchConfigurationKey;
        private IEnumerable<object>? _configurationItems;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _configurationItems = ["abc", "def", "ghi"];
        }
    }
}
