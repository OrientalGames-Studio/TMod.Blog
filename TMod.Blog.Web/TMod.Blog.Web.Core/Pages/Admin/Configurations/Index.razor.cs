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

        internal DateRange? CreateDateRange
        {
            get => _createDateRange;
            set
            {
                if ( _createDateRange != value )
                {
                    _createDateRange = value;
                    if ( _dataGrid is not null )
                    {
                        InvokeAsync(_dataGrid.ReloadServerData);
                    }
                }
            }
        }

        internal string? SearchConfigurationKey
        {
            get => _searchConfigurationKey;
            set
            {
                if ( _searchConfigurationKey != value )
                {
                    _searchConfigurationKey = value;
                    if ( _dataGrid is not null )
                    {
                        InvokeAsync(_dataGrid.ReloadServerData);
                    }
                }
            }
        }

        [Inject]
        public IAppConfigurationProviderService? AppConfigurationProviderService { get; set; }

        [Inject]
        public IDialogService? DialogService { get; set; }

        [Inject]
        public ISnackbar? Snackbar { get; set; }

        private async Task<GridData<ConfigurationViewModel?>> QueryDataAsync(GridState<ConfigurationViewModel?> gridState)
        {
            var pagingResult = await AppConfigurationProviderService!.GetAllConfigurationsAsync(gridState.PageSize,gridState.Page,SearchConfigurationKey,CreateDateRange?.Start is null?null:DateOnly.FromDateTime(CreateDateRange.Start.Value),CreateDateRange?.End is null?null:DateOnly.FromDateTime(CreateDateRange.End.Value));
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

        private async Task OnBatchRemoveButtonClickAsync(MouseEventArgs e)
        {
            if(e.Detail > 1 || _selectedConfigurations is null || _selectedConfigurations.Count == 0 )
            {
                return;
            }
            await BatchRemoveConfigurationByIdAsync(_selectedConfigurations?.ToList() ?? []);
        }

        private async Task OnRemoveButtonClickAsync(MouseEventArgs e,CellContext<ConfigurationViewModel?> viewModelContext)
        {
            if(e.Detail > 1 || viewModelContext is null || viewModelContext.Item is null )
            {
                return;
            }
            await BatchRemoveConfigurationByIdAsync([viewModelContext.Item]);
        }

        private async Task BatchRemoveConfigurationByIdAsync(List<ConfigurationViewModel?> configurations)
        {
            if ( configurations is null || configurations.Count == 0 )
            {
                return;
            }
            bool? confirm = await DialogService!.ShowMessageBox("删除确认", $"是否要删除{( configurations.Count == 1 ? $"{configurations.First()?.Key}这一条" : $"{string.Join(",", configurations.Select(c => c?.Key))}这几条" )}配置项？", "是", "否", options: new DialogOptions()
            {
                DisableBackdropClick = true,
            });
            if(confirm == true )
            {
                await AppConfigurationProviderService!.BatchRemoveConfigurationByIdAsync(configurations.Select(p => p!.Id).ToArray());
                Snackbar!.Add("删除成功");
                await InvokeAsync(_dataGrid!.ReloadServerData);
            }
        }
	}
}
