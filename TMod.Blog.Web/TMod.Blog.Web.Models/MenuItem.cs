﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TMod.Blog.Web.Models
{
    public sealed class MenuItem
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string Url { get; set; } = "/";

        public string? Icon { get; set; }

        public string? IconUrl { get; set; }

        public bool Disabled { get; set; } = false;

        public bool Visible { get; set; } = true;

        public ICollection<MenuItem>? SubMenus { get; set; } = new List<MenuItem>();

        public MenuItem(string? title,string? url,params MenuItem[] subMenus)
        {
            Title = title;
            Description = title;
            Url = url ?? "/";
            SubMenus = new List<MenuItem>(subMenus);
        }

        public MenuItem(string? title,string? url,string? description = null,string? icon = null,string? iconUrl = null,bool disabled = false,bool visible = true, params MenuItem[] subMenus)
        {
            Title = title;
            Description = description ?? title;
            Url = url ?? "/";
            Icon = icon;
            IconUrl = iconUrl;
            Disabled = disabled;
            Visible = visible;
            SubMenus = new List<MenuItem>(subMenus);
        }
        public static MenuItem HomeMenuItem => new MenuItem("主页", "/","博客主业", """
             <?xml version="1.0" encoding="utf-8"?>
            <!-- License: MIT. Made by uiwjs: https://github.com/uiwjs/icons -->
            <svg viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
              <path d="M18.1780455,11.3733043 C18.5648068,11.3733043 18.8783387,11.6865112 18.8783387,12.0728715 L18.8779659,17.9472709 C18.9045058,18.7594781 18.8070167,19.291671 18.4437066,19.6266035 C18.105756,19.9381573 17.6163877,20.02774 16.9806726,19.9929477 L3.14358762,19.9921612 C2.50135353,19.9617139 2.00101685,19.6995595 1.76807877,19.1740143 C1.61416876,18.826769 1.54233858,18.4172981 1.54233858,17.9457519 L1.54233858,12.0728715 C1.54233858,11.6865112 1.85587053,11.3733043 2.2426318,11.3733043 C2.62939306,11.3733043 2.94292502,11.6865112 2.94292502,12.0728715 L2.94292502,17.9457519 C2.94292502,18.1775916 2.96768644,18.3651131 3.01196148,18.5083933 L3.048,18.606 L3.04520125,18.5951633 C3.04625563,18.5832362 3.07436346,18.5883277 3.17678453,18.5938132 L17.0178944,18.5948021 C17.2619058,18.6077766 17.4181773,18.5935806 17.4731456,18.5929701 L17.477,18.592 C17.4642876,18.5389055 17.4889447,18.3217509 17.4777523,17.9700942 L17.4777523,12.0728715 C17.4777523,11.6865112 17.7912843,11.3733043 18.1780455,11.3733043 Z M10.4342636,0 C10.6979883,0 10.9335521,0.103647698 11.156261,0.297113339 L19.7806041,8.43584529 C20.0617527,8.70116319 20.0743627,9.14392549 19.8087695,9.42478257 C19.5431762,9.70563964 19.0999544,9.71823663 18.8188059,9.45291873 L10.4018236,1.50898373 L1.15769646,9.47411857 C0.864827408,9.72646706 0.422628974,9.69386478 0.170018608,9.40129935 C-0.082591757,9.10873393 -0.049955647,8.66699392 0.2429134,8.41464544 L9.6885128,0.275913491 L9.77478626,0.212395396 C9.98943808,0.0783954693 10.2025363,0 10.4342636,0 Z"/>
            </svg>
            """);
    }

    public static class MenuItemExtensions
    {
        public static MenuItem? FirstOrDefault(this IEnumerable<MenuItem> menuItems,string? url)
        {
            url = string.IsNullOrWhiteSpace(url) ? "/" : url;
            foreach ( MenuItem menuItem in menuItems )
            {
                if ( UrlMatches(menuItem.Url, url) )
                {
                    return menuItem;
                }
                if(menuItem.SubMenus is not null && menuItem.SubMenus.Count > 0 )
                {
                    MenuItem? subMenu = menuItem.SubMenus.FirstOrDefault(url);
                    if(subMenu is not null )
                    {
                        return subMenu;
                    }
                }
            }
            return null;
        }

        private static bool UrlMatches(string pattern, string url)
        {
            string regexPattern = $"^{(Regex.Escape(pattern).Replace("\\{","{").Replace("{","(.*?)").Replace("}",""))}$";
            return Regex.IsMatch(url, regexPattern,RegexOptions.IgnoreCase);
        }
    }
}
