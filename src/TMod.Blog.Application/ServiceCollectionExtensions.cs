using FluentValidation;

using Mapster;

using MapsterMapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Services;
using TMod.Blog.Application.Services.Implements;

namespace TMod.Blog.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlogApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining(typeof(ServiceCollectionExtensions),includeInternalTypes:true);

            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan([typeof(ServiceCollectionExtensions).Assembly]);
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            AddBlogServices(services);
            return services;
        }

        private static void AddBlogServices(IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IArticleService, ArticleService>());
            services.TryAddEnumerable(ServiceDescriptor.Scoped<ICategoryService, CategoryService>());
            services.TryAddEnumerable(ServiceDescriptor.Scoped<ICommentService, CommentService>());
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IFavoriteService, FavoriteService>());
            services.TryAddEnumerable(ServiceDescriptor.Scoped<ISiteConfigurationService, SiteConfigurationService>());
            services.TryAddEnumerable(ServiceDescriptor.Scoped<ITagService, TagService>());
        }
    }
}
