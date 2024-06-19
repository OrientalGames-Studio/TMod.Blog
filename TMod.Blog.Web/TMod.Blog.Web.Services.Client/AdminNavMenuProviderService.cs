using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Web.Models;
using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Services.Client
{
    internal class AdminNavMenuProviderService : INavMenuProviderService
    {
        private readonly IIconPathProviderService _iconPathProviderService;
        private readonly MenuItem[] _basicMenuItems = [];
        private readonly MenuItem[] _articleMenuItems = [];

        public AdminNavMenuProviderService(IIconPathProviderService iconPathProviderService)
        {
            _iconPathProviderService = iconPathProviderService;
            _basicMenuItems = [
            MenuItem.HomeMenuItem
            ,new MenuItem("管理员仪表盘","/admin",icon: _iconPathProviderService.NavMenu_Admin_DashboardIcon)];
            _articleMenuItems = [new MenuItem("文章管理",null,null,icon:_iconPathProviderService.ListIcon,null,false,true,[
                new MenuItem("文章列表","/admin/articles","博客文章列表",_iconPathProviderService.NavMenu_Admin_ArticlesIcon,null,false,true,[
                    new MenuItem("编辑文章","/admin/articles/{articleId}/edit","编辑博客文章",icon:_iconPathProviderService.EditIcon,visible:false),
                    new MenuItem("查看文章","/admin/articles/{articleId}","查看博客文章",icon:_iconPathProviderService.PreviewIcon,visible:false)
                    ])
                ])
            ,new MenuItem("文章分类管理",null,null,icon:_iconPathProviderService.ListIcon,null,false,true,[
                new MenuItem("文章分类列表","/admin/categories","所有文章可以选择的分类列表",icon:_iconPathProviderService.NavMenu_Admin_CategoryIcon,null,false,true,[
                    new MenuItem("编辑分类","/admin/categories/{categoryId}/edit","编辑分类",icon:_iconPathProviderService.EditIcon,visible:false)
                    ])
                ])
            ,new MenuItem("配置管理","/admin/configurations","应用配置项列表",icon:_iconPathProviderService.ConfigurationIcon,null,false,true)
            ,new MenuItem("报表数据",null,null,icon:_iconPathProviderService.ReportIcon,null,false,true,[
                new MenuItem("报表1","/admin/reports",null,_iconPathProviderService.ReportIcon),
                new MenuItem("报表2","/admin/reports",null,_iconPathProviderService.ReportIcon),
                new MenuItem("报表3","/admin/reports",null,_iconPathProviderService.ReportIcon),
                ])
            ];
        }

        public IEnumerable<MenuItem> GetNavMenuItems()
        {
            return [.. _basicMenuItems,.._articleMenuItems];
        }
    }
}
