using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Configuration;
using TMod.Blog.Web.Core.Components.Configurations;
using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Core.Pages.Admin.Configurations
{
    public partial class Index:ComponentBase
    {
        private DateRange? _createDateRange;
        private string? _searchConfigurationKey;
        private HashSet<ConfigurationViewModel?>? _selectedConfigurations;
        private MudDataGrid<ConfigurationViewModel?>? _dataGrid;

        [Inject]
        public IAppConfigurationProviderService? AppConfigurationProviderService { get; set; }

        [Inject]
        public IDialogService? DialogService { get; set; }

        private async Task<GridData<ConfigurationViewModel?>> QueryDataAsync(GridState<ConfigurationViewModel?> gridState)
        {
            var pagingResult = await AppConfigurationProviderService!.GetAllConfigurations(gridState.PageSize,gridState.Page,_searchConfigurationKey,_createDateRange?.Start is null?null:DateOnly.FromDateTime(_createDateRange.Start.Value),_createDateRange?.End is null?null:DateOnly.FromDateTime(_createDateRange.End.Value));
            return new GridData<ConfigurationViewModel?>()
            {
                Items = pagingResult.Data,
                TotalItems = pagingResult.DataCount
            };
        }

        private async Task OnEditButtonClickAsync(MouseEventArgs e,CellContext<ConfigurationViewModel?> viewModelContext)
        {
            if(e.Detail > 1 )
            {
                return;
            }
            if(viewModelContext is not null && viewModelContext.Item is not null)
            {
                await ShowEditDialogAsync(viewModelContext.Item);
            }
        }

        private async Task OnAddButtonClickAsync(MouseEventArgs e)
        {
            if(e.Detail > 1 )
            {
                return;
            }
            await ShowEditDialogAsync(null);
        }


        private async Task ShowEditDialogAsync(ConfigurationViewModel? viewModel)
        {
            DialogOptions options = new DialogOptions()
            {
                DisableBackdropClick = true,
                FullWidth = true,
                Position = DialogPosition.Center,
                CloseButton = true,
            };
            DialogParameters<EditConfigurationDialog> parameters = new DialogParameters<EditConfigurationDialog>
            {
                { p => p.Model, viewModel }
            };
            IDialogReference dialog = await DialogService!.ShowAsync<EditConfigurationDialog>($"{( viewModel is null ? "新增" : "编辑" )}配置项", parameters, options);
            if(!(await dialog.Result ).Canceled )
            {
                await InvokeAsync(_dataGrid!.ReloadServerData);
            }
        }
	}
}
