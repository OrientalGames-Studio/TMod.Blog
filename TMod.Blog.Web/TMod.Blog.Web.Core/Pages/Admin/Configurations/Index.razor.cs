using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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
        private HashSet<ConfigurationViewModel?>? _selectedConfigurations;

        [Inject]
        public IAppConfigurationProviderService? AppConfigurationProviderService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			await base.OnAfterRenderAsync(firstRender);
		}

        private async Task<GridData<ConfigurationViewModel?>> QueryDataAsync(GridState<ConfigurationViewModel?> gridState)
        {
            var pagingResult = await AppConfigurationProviderService!.GetAllConfigurations(gridState.PageSize,gridState.Page,_searchConfigurationKey,_createDateRange?.Start is null?null:DateOnly.FromDateTime(_createDateRange.Start.Value),_createDateRange?.End is null?null:DateOnly.FromDateTime(_createDateRange.End.Value));
            return new GridData<ConfigurationViewModel?>()
            {
                Items = pagingResult.Data,
                TotalItems = pagingResult.DataCount
            };
        }

        private void OnSelectedConfigurationsChanged(HashSet<ConfigurationViewModel?> selectedConfigurations)
        {
            _selectedConfigurations = selectedConfigurations;
            StateHasChanged();
        }

        private void OnSearchBoxKeyDown(KeyboardEventArgs e)
        {
            ;
        }

        private void OnSearchBoxSearch(MouseEventArgs key)
        {
            return;
        }

        private void OnDateRangeChanged(DateRange dateRange)
        {

        }
	}
}
