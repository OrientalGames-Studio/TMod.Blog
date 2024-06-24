using Microsoft.AspNetCore.Components;

using MudBlazor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Configuration;
using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Core.Pages.Admin.Configurations
{
    public partial class Index:ComponentBase
    {
        private DateRange? _createDateRange;
        private string? _searchConfigurationKey;
        private int _pageIndex = 1;
        private int _pageSize = 20;
        private int _dataCount = 0;
        private int _pageCount = 1;
        private IEnumerable<ConfigurationViewModel?>? _configurationItems;

        [Inject]
        public IAppConfigurationProviderService? AppConfigurationProviderService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			await base.OnAfterRenderAsync(firstRender);
			var pagingResult = await AppConfigurationProviderService!.GetAllConfigurations(_pageSize,_pageIndex);
			_pageIndex = pagingResult.PageIndex;
			_pageSize = pagingResult.PageSize;
			_dataCount = pagingResult.DataCount;
			_pageCount = pagingResult.PageCount;
            _configurationItems = pagingResult.Data;
		}
	}
}
