using FluentValidation;

using Mapster;

using MapsterMapper;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlogApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining(typeof(ServiceCollectionExtensions));

            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan([typeof(ServiceCollectionExtensions).Assembly]);
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            return services;
        }
    }
}
