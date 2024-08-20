using Microsoft.AspNetCore.Components;

using MudBlazor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Models.ViewModels.Articles;
using TMod.Blog.Data.Models.ViewModels.Categories;
using TMod.Blog.Web.Models;
using TMod.Blog.Web.Models.Articles;
using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Core.Pages.Admin.Articles
{
    public partial class Index : ComponentBase
    {
        private DateRange? _createDateRange;
        private DateRange? _editDateRange;
        private DateRange? _publishDateRange;
        private string? _articleTitleFilter;
        private string? _articleContentFilter;
        private bool? _articleCommentStateFilter;
        private IEnumerable<CategoryViewModel?> _selectedCategories = [];
        private IEnumerable<string?> _selectedTags = [];
        private bool _isDraft,_isPublished,_isHidden;
        private MudDataGrid<ArticleViewModel> _dataGrid;

        private IEnumerable<CategoryViewModel?> _enabledCategories = [];
        private IEnumerable<string?> _enabledTags = [];
        private HashSet<ArticleViewModel?>? _selectedArticles;

        [Inject]
        public ICategoryService? CategoryService { get; set; }

        [Inject]
        public ITagService? TagService { get; set; }

        [Inject]
        public IArticleService? ArticleService { get; set; }

        [Inject]
        public ISnackbar? Snackbar { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await InitializedFilterDataAsync();
        }

        private async Task InitializedFilterDataAsync()
        {
            InitializedArticleStateFilter();
            Task categoriesTask = InitializedEnabledCategoriesAsync();
            Task tagsTask = InitializedEnabledTagsAsync();
            await Task.WhenAll([categoriesTask, tagsTask]);
        }

        private async Task InitializedEnabledTagsAsync()
        {
            // 请求标签服务查询所有标签
            IEnumerable<string?> allTags = await TagService!.GetTagsAsync();
            if ( allTags.Any() )
            {
                _enabledTags = allTags;
                // 默认筛选所有标签的数据
                _selectedTags = allTags;
            }
        }

        private async Task InitializedEnabledCategoriesAsync()
        {
            // 请求分类服务查询所有分类
            PagingResult<CategoryViewModel?> pagingResult = await CategoryService!.GetAllCategoriesAsync(int.MaxValue);
            if ( pagingResult.DataCount > 0 )
            {
                _enabledCategories = pagingResult.Data;
                // 默认筛选所有分类的数据
                _selectedCategories = pagingResult.Data;
            }
        }

        private void InitializedArticleStateFilter()
        {
            // 默认筛选所有状态的文章数据，所以三个 CheckBox 默认勾上
            _isDraft = true;
            _isPublished = true;
            _isHidden = true;
        }

        private static string GetCategoryMultiSelectionText(List<string?> selections)
        {
            if ( selections is null || selections.Count < 1 )
            {
                return string.Empty;
            }
            return $"已选择了{selections.Count}个分类";
        }

        private static string GetTagMultiSelectionText(List<string?> selections)
        {
            if ( selections is null || selections.Count < 1 )
            {
                return string.Empty;
            }
            return $"已选择了{selections.Count}个标签";
        }

        private async Task<GridData<ArticleViewModel?>> QueryDataAsync(GridState<ArticleViewModel?> gridState)
        {
            _selectedArticles?.Clear();
            QueryArticleFilterModel filterModel = new QueryArticleFilterModel()
            {
                ArticleTitleFilter = _articleTitleFilter,
                ArticleSnapshotFilter = _articleContentFilter,
                ArticleCategoryFilter = _selectedCategories.Select(p=>p?.Category),
                ArticleTagFilter = _selectedTags,
                ArticleCommentStateFilter = _articleCommentStateFilter,
                MinArticleCreateDate =    (_createDateRange?.Start.HasValue==true)?DateOnly.FromDateTime(_createDateRange.Start.GetValueOrDefault()):null,
                MaxArticleCreateDate =    (_createDateRange?.End.HasValue == true)?DateOnly.FromDateTime(_createDateRange.End.GetValueOrDefault()):null,
                MinArticleLastEditDate =  (_editDateRange?.Start.HasValue == true)?DateOnly.FromDateTime(_editDateRange.Start.GetValueOrDefault()):null,
                MaxArticleLastEditDate =  (_editDateRange?.End.HasValue == true)?DateOnly.FromDateTime(_editDateRange.End.GetValueOrDefault()):null,
                MinArticlePublishedDate = (_publishDateRange?.Start.HasValue == true)?DateOnly.FromDateTime(_publishDateRange.Start.GetValueOrDefault()):null,
                MaxArticlePublishedDate = (_publishDateRange?.End.HasValue == true)?DateOnly.FromDateTime(_publishDateRange.End.GetValueOrDefault()):null
            };
            ArticleStateEnum? stateEnum = null;
            if ( _isDraft )
            {
                AppendArticleState(ref stateEnum, ArticleStateEnum.Draft);
            }
            if ( _isPublished )
            {
                AppendArticleState(ref stateEnum, ArticleStateEnum.Published);
            }
            if ( _isHidden )
            {
                AppendArticleState(ref stateEnum, ArticleStateEnum.Hidden);
            }
            filterModel.ArticleStateFilter = ( short? )stateEnum;
            PagingResult<ArticleViewModel?>? pagingResult = await ArticleService!.GetAllArticleByPaging(gridState.PageSize,filterModel,gridState.Page);
            return new GridData<ArticleViewModel?>()
            {
                Items = pagingResult.Data,
                TotalItems = pagingResult.DataCount
            };
        }
        private void AppendArticleState(ref ArticleStateEnum? stateEnum, ArticleStateEnum state)
        {
            if ( stateEnum is null )
            {
                stateEnum = state;
            }
            else
            {
                stateEnum |= state;
            }
        }

        private async Task OnArticleCommentIsEnabledChangedAsync(CellContext<ArticleViewModel> article, bool isCommentEnabled)
        {
            ArticleViewModel? metaData = await ArticleService!.UpdateArticleCommentIsEnabledAsync(article.Item.Id, isCommentEnabled);
            if ( metaData is not null )
            {
                await _dataGrid.ReloadServerData();
            }
        }

        private async Task BatchChangeArticleCommentIsEnabledAsync()
        {
            if(_selectedArticles is null || _selectedArticles.Count == 0 )
            {
                return;
            }
            Dictionary<Guid,bool> dic = new Dictionary<Guid, bool>();
            foreach ( ArticleViewModel? article in _selectedArticles )
            {
                if(article is null )
                {
                    continue;
                }
                dic[article.Id] = !article.IsCommentEnabled;
            }
            bool result = await ArticleService!.BatchUpdateArticleCommentIsEnabledAsync(dic);
            if ( result )
            {
                Snackbar!.Add("修改成功", Severity.Success);
                _dataGrid?.ReloadServerData();
            }
            else
            {
                Snackbar!.Add("修改失败，请重试", Severity.Warning);
            }
        }
    }
}
