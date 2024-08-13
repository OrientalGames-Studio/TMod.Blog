using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Categories;

using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Core.Components.Categories
{
    public partial class EditCategoryDialog:ComponentBase
    {
        private bool _isEdit;
        private MudForm? _form;
        private string? _originCategory;

        [CascadingParameter]
        public MudDialogInstance? MudDialogInstance { get; set; }

        [Parameter]
        public CategoryViewModel? Model { get; set; }

        [Inject]
        public ICategoryService? CategoryService { get; set; }

        [Inject]
        public ISnackbar? Snackbar { get; set; }

        internal string ConfirmButtonText => $"{( _isEdit ? "保存" : "创建" )}分类";

        internal string CancelButtonText => $"取消{( _isEdit ? "保存" : "创建" )}";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if ( Model is null )
            {
                Model = new CategoryViewModel();
                _isEdit = false;
            }
            else
            {
                _isEdit = true;
                _originCategory = Model.Category;
            }
        }

        private async Task OnSaveButtonClickAsync(MouseEventArgs e)
        {
            if ( e.Detail > 1 || _form is null )
            {
                return;
            }
            await _form.Validate();
            if ( !_form.IsValid )
            {
                return;
            }
            Task task = CategoryService!.SaveCategoryAsync(_originCategory,Model!.Category);
            await task;
            if ( task.IsFaulted )
            {
                Snackbar!.Add($"保存分类失败:{task.Exception?.Message}", Severity.Error);
                return;
            }
            MudDialogInstance?.Close(DialogResult.Ok<object?>(null));
        }
    }
}
