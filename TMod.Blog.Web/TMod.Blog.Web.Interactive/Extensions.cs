using Microsoft.Extensions.DependencyInjection;
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
    }
}
