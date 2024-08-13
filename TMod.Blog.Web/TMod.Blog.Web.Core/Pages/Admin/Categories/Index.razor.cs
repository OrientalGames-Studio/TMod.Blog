using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor;

using OneOf.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Categories;
using TMod.Blog.Web.Core.Components.Categories;
using TMod.Blog.Web.Core.Components.Configurations;
using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Core.Pages.Admin.Categories
{
    public partial class Index : ComponentBase
    {
        private DateRange? _createDateRange;
        private string? _searchCategoryKey;

        private HashSet<CategoryViewModel?>? _selectedCategories;
        private MudDataGrid<CategoryViewModel?>? _dataGrid;

        internal DateRange? CreateDateRange
        {
            get => _createDateRange;
            set
            {
                if(_createDateRange != value )
                {
                    _createDateRange = value;
                    if(_dataGrid is not null )
                    {
                        InvokeAsync(_dataGrid.ReloadServerData);
                    }
                }
            }
        }

        internal string? SearchCategoryKey
        {
            get => _searchCategoryKey;
            set
            {
                if(_searchCategoryKey != value )
                {
                    _searchCategoryKey = value;
                    if ( _dataGrid is not null )
                    {
                        InvokeAsync(_dataGrid.ReloadServerData);
                    }
                }
            }
        }

        [Inject]
        public ICategoryService? CategoryService { get; set; }

        [Inject]
        public IDialogService? DialogService { get; set; }

        [Inject]
        public ISnackbar? Snackbar { get; set; }

        private async Task<GridData<CategoryViewModel?>> QueryDataAsync(GridState<CategoryViewModel?> gridState)
        {
            var pagingResult = await CategoryService!.GetAllCategoriesAsync(gridState.PageSize,gridState.Page,SearchCategoryKey,CreateDateRange?.Start is null?null:DateOnly.FromDateTime(CreateDateRange.Start.Value),CreateDateRange?.End is null?null:DateOnly.FromDateTime(CreateDateRange.End.Value));
            return new GridData<CategoryViewModel?>()
            {
                Items = pagingResult.Data,
                TotalItems = pagingResult.DataCount
            };
        }

        private async Task OnAddButtonClickAsync(MouseEventArgs e)
        {
            if(e.Detail > 1 )
            {
                return;
            }
            await ShowEditCategoryDialogAsync(null);
        }

        private async Task ShowEditCategoryDialogAsync(CategoryViewModel? viewModel)
        {
            DialogOptions options = new DialogOptions()
            {
                DisableBackdropClick = true,
                FullWidth = true,
                Position = DialogPosition.Center,
                CloseButton = true,
            };
            DialogParameters<EditCategoryDialog> parameters = new DialogParameters<EditCategoryDialog>
            {
                { p => p.Model, viewModel }
            };
            IDialogReference dialog = await DialogService!.ShowAsync<EditCategoryDialog>($"{( viewModel is null ? "新增" : "编辑" )}分类", parameters, options);
            if ( !( await dialog.Result ).Canceled )
            {
                await InvokeAsync(_dataGrid!.ReloadServerData);
            }
        }

        private async Task OnEditButtonClickAsync(MouseEventArgs e,CellContext<CategoryViewModel?> context)
        {
            if(e.Detail > 1 )
            {
                return;
            }
            if(context is not null && context.Item is not null )
            {
                await ShowEditCategoryDialogAsync(context.Item);
            }
        }

        private async Task OnRemoveButtonClickAsync(MouseEventArgs e,CellContext<CategoryViewModel?> context)
        {
            if(e.Detail > 1 || context is null || context.Item is null )
            {
                return;
            }
            await BatchRemoveCategoryByIdAsync([context.Item]);
        }

        private async Task BatchRemoveCategoryByIdAsync(List<CategoryViewModel?> categories)
        {
            if(categories is null || categories.Count == 0 )
            {
                return;
            }
            bool? confirm = await DialogService!.ShowMessageBox("删除确认",$"是否要删除{(categories.Count == 1 ? $"{categories.First()?.Category}这一个":$"{string.Join(",",categories.Select(p=>p?.Category))}这几个")}分类？","是","否",options: new DialogOptions()
            {
                DisableBackdropClick = true,
            });
            if(confirm == true )
            {
                await CategoryService!.BatchRemoveCategoryByIdAsync(categories.Select(p => p!.Id).ToArray());
                Snackbar!.Add("删除成功");
                await InvokeAsync(_dataGrid!.ReloadServerData);
            }
        }

        private async Task OnBatchRemoveButtonClickAsync(MouseEventArgs e)
        {
            if(e.Detail > 1 || _selectedCategories is null || _selectedCategories.Count == 0 )
            {
                return;
            }
            await BatchRemoveCategoryByIdAsync(_selectedCategories?.ToList() ?? []);
        }
    }
}
