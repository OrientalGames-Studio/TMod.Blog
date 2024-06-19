using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

using MudBlazor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Web.Models;

namespace TMod.Blog.Web.Core.Components
{
    public partial class BlogBreadCrumbs : ComponentBase, IDisposable
    {
        private List<BreadcrumbItem>? _breadcrumbItems;

        [Parameter]
        public IEnumerable<MenuItem>? MenuItems { get; set; }

        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            CreateEmptyBreadcurmbList(NavigationManager!.ToBaseRelativePath(NavigationManager.BaseUri));
            if ( NavigationManager is not null )
            {
                NavigationManager.LocationChanged += OnLocationChanged;
            }
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            string? breadcurbUrl = NavigationManager?.ToBaseRelativePath(e.Location);
            SetBreadcrumb(breadcurbUrl);
        }

        private void CreateEmptyBreadcurmbList(string? initialUrl)
        {
            initialUrl = string.IsNullOrWhiteSpace(initialUrl) ? "/" : initialUrl;
            MenuItem? menuItem = MenuItems?.FirstOrDefault(initialUrl);
            if ( menuItem is not null )
            {
                _breadcrumbItems = [new BreadcrumbItem(menuItem.Title!, menuItem.Url, false, menuItem.Icon)];
            }
        }

        private void SetBreadcrumb(string? url)
        {
            url = string.IsNullOrWhiteSpace(url) ? "/" : url;
            if ( !url.StartsWith("/") )
            {
                url = url.TrimStart(['/', '\\']).Insert(0, "/");
            }
            if ( _breadcrumbItems?.Any(p => p.Href == url) == true )
            {
                int index = _breadcrumbItems.FindIndex(p => p.Href == url);
                int pos = _breadcrumbItems.Count;
                do
                {
                    _breadcrumbItems.Remove(_breadcrumbItems[^1]);
                    pos--;
                } while ( pos > index );
            }
            MenuItem? menuItem = MenuItems?.FirstOrDefault(url);
            if ( menuItem is not null )
            {
                _breadcrumbItems?.Add(new BreadcrumbItem(menuItem.Title!, menuItem.Url, false, menuItem.Icon));
            }
            StateHasChanged();
        }

        public void Dispose()
        {
            if ( NavigationManager is not null )
            {
                NavigationManager.LocationChanged -= OnLocationChanged;
            }
        }
    }
}
