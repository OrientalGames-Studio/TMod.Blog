using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Web.Services.Abstraction;
using TMod.Blog.Web.Services.Client;
using TMod.Blog.Web.Interactive;

namespace TMod.Blog.Web.Services
{
    public static class Extensions
    {
        public static IServiceCollection AddIconPathProviderService(this IServiceCollection services)
        {
            services.AddSingleton<IIconPathProviderService, IconPathProviderService>();
            services.AddCascadingValue<IIconPathProviderService>(provider => provider.GetRequiredService<IIconPathProviderService>());
            return services;
        }

        public static IServiceCollection AddAppConfigurationProviderService(this IServiceCollection services)
        {
            services.AddAppConfigurationApi();
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IAppConfigurationProviderService, AppConfigurationProviderService>());
            services.AddCascadingValue<IAppConfigurationProviderService>(provider => provider.GetRequiredService<IAppConfigurationProviderService>());
            return services;
        }

        public static IServiceCollection AddLocalStorageProviderService(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Scoped<ILocalStorageProviderService, LocalStorageProviderService>());
            return services;
        }
    }
}
