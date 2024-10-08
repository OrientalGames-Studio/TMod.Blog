﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Web.Interactive.Abstraction;

namespace TMod.Blog.Web.Interactive
{
    public static class Extensions
    {
        public static IServiceCollection AddAppConfigurationApi(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IAppConfigurationApi, AppConfigurationApi>());
            return services;
        }

        public static IServiceCollection AddCategoryApi(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ICategoryApi, CategoryApi>());
            return services;
        }

        public static IServiceCollection AddTagApi(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITagApi, TagApi>());
            return services;
        }

        public static IServiceCollection AddArticleApi(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IArticleApi, ArticleApi>());
            return services;
        }
    }
}
